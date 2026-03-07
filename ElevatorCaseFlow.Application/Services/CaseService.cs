using ElevatorCaseFlow.Application.DTOs;
using ElevatorCaseFlow.Application.Interfaces;
using ElevatorCaseFlow.Domain.Entities;
using ElevatorCaseFlow.Domain.Enums;

namespace ElevatorCaseFlow.Application.Services
{
    /// <summary>
    /// Handles all business logic for elevator case operations.
    /// Sits between the API controllers and the database repository.
    /// </summary>
    public class CaseService : ICaseService
    {
        // Repository is injected — service never touches DB directly
        private readonly ICaseRepository _caseRepository;

        public CaseService(ICaseRepository caseRepository)
        {
            _caseRepository = caseRepository;
        }

        /// <summary>
        /// Returns all elevator cases from the database.
        /// </summary>
        public async Task<IEnumerable<CaseResponse>> GetAllCasesAsync()
        {
            var cases = await _caseRepository.GetAllAsync();

            // Convert each domain entity to a response DTO
            return cases.Select(MapToResponse);
        }

        /// <summary>
        /// Returns a single case by ID. Returns null if not found.
        /// </summary>
        public async Task<CaseResponse?> GetCaseByIdAsync(int id)
        {
            var elevatorCase = await _caseRepository.GetByIdAsync(id);

            // Return null if case doesn't exist
            if (elevatorCase == null) return null;

            return MapToResponse(elevatorCase);
        }

        /// <summary>
        /// Creates a new elevator case and saves it to the database.
        /// Auto-generates a unique case number.
        /// </summary>
        public async Task<CaseResponse> CreateCaseAsync(CreateCaseRequest request)
        {
            // Map the incoming request DTO to a domain entity
            var newCase = new ElevatorCase
            {
                // Generate a unique case number using timestamp
                CaseNumber = $"CASE-{DateTime.UtcNow:yyyyMMddHHmmss}",
                ClientName = request.ClientName,
                Country = request.Country,
                BuildingType = request.BuildingType,
                FloorCount = request.FloorCount,
                XmlPayload = request.XmlPayload,

                // Every new case starts as Submitted
                Status = CaseStatus.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _caseRepository.CreateAsync(newCase);

            return MapToResponse(created);
        }

        /// <summary>
        /// Updates the workflow status of an existing case.
        /// Optionally records a rejection reason if status is Rejected.
        /// </summary>
        public async Task<CaseResponse?> UpdateCaseStatusAsync(
            int id, CaseStatus status, string? rejectionReason = null)
        {
            var updated = await _caseRepository.UpdateStatusAsync(id, status, rejectionReason);

            if (updated == null) return null;

            return MapToResponse(updated);
        }

        /// <summary>
        /// Runs business validation rules against a submitted case.
        /// Automatically updates the case status to Validated or Rejected.
        /// </summary>
        public async Task<(bool IsValid, string Message)> ValidateCaseAsync(int id)
        {
            var elevatorCase = await _caseRepository.GetByIdAsync(id);

            // Cannot validate a case that doesn't exist
            if (elevatorCase == null)
                return (false, "Case not found.");

            // Can only validate cases that are in Submitted status
            if (elevatorCase.Status != CaseStatus.Submitted)
                return (false, $"Case is already in '{elevatorCase.Status}' status.");

            // ── Business Validation Rules ──

            // Rule 1: Client name must be provided
            if (string.IsNullOrWhiteSpace(elevatorCase.ClientName))
            {
                await _caseRepository.UpdateStatusAsync(id, CaseStatus.Rejected,
                    "Client name is required.");
                return (false, "Validation failed: Client name is required.");
            }

            // Rule 2: Floor count must be at least 2
            if (elevatorCase.FloorCount < 2)
            {
                await _caseRepository.UpdateStatusAsync(id, CaseStatus.Rejected,
                    "Floor count must be at least 2.");
                return (false, "Validation failed: Floor count must be at least 2.");
            }

            // Rule 3: Floor count cannot exceed 200 (engineering limit)
            if (elevatorCase.FloorCount > 200)
            {
                await _caseRepository.UpdateStatusAsync(id, CaseStatus.Rejected,
                    "Floor count cannot exceed 200.");
                return (false, "Validation failed: Floor count cannot exceed 200.");
            }

            // Rule 4: XML payload must be present
            if (string.IsNullOrWhiteSpace(elevatorCase.XmlPayload))
            {
                await _caseRepository.UpdateStatusAsync(id, CaseStatus.Rejected,
                    "XML payload is required for processing.");
                return (false, "Validation failed: XML payload is required.");
            }

            // All rules passed — mark case as Validated
            await _caseRepository.UpdateStatusAsync(id, CaseStatus.Validated);

            return (true, "Case validated successfully. Ready for processing.");
        }

        /// <summary>
        /// Deletes a case by ID. Returns true if deleted, false if not found.
        /// </summary>
        public async Task<bool> DeleteCaseAsync(int id)
        {
            return await _caseRepository.DeleteAsync(id);
        }

        // ── Private Helper ──

        /// <summary>
        /// Maps a domain ElevatorCase entity to a CaseResponse DTO.
        /// Keeps domain objects from leaking out of the service layer.
        /// </summary>
        private static CaseResponse MapToResponse(ElevatorCase c) => new()
        {
            Id = c.Id,
            CaseNumber = c.CaseNumber,
            ClientName = c.ClientName,
            Country = c.Country,
            BuildingType = c.BuildingType,
            FloorCount = c.FloorCount,
            Status = c.Status.ToString(), // Convert enum to readable string
            RejectionReason = c.RejectionReason,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}

