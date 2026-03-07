using ElevatorCaseFlow.Domain.Entities;
using ElevatorCaseFlow.Domain.Enums;

namespace ElevatorCaseFlow.Application.Interfaces
{
    /// <summary>
    /// Contract for all database operations related to ElevatorCases.
    /// The actual implementation lives in the Infrastructure layer.
    /// </summary>
    public interface ICaseRepository
    {
        // Get all cases from the database
        Task<IEnumerable<ElevatorCase>> GetAllAsync();

        // Get a single case by its ID
        Task<ElevatorCase?> GetByIdAsync(int id);

        // Get a single case by its case number (e.g. CASE-00001)
        Task<ElevatorCase?> GetByCaseNumberAsync(string caseNumber);

        // Save a new case to the database
        Task<ElevatorCase> CreateAsync(ElevatorCase elevatorCase);

        // Update the status of an existing case
        Task<ElevatorCase?> UpdateStatusAsync(int id, CaseStatus status, string? rejectionReason = null);

        // Delete a case by ID — returns true if deleted, false if not found
        Task<bool> DeleteAsync(int id);
    }
}