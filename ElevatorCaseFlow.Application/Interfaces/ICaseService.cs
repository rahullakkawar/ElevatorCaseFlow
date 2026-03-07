using ElevatorCaseFlow.Application.DTOs;
using ElevatorCaseFlow.Domain.Enums;

namespace ElevatorCaseFlow.Application.Interfaces
{
    /// <summary>
    /// Contract for all business logic operations related to ElevatorCases.
    /// The actual implementation is in CaseService.cs in the Services folder.
    /// </summary>
    public interface ICaseService
    {
        // Get all cases
        Task<IEnumerable<CaseResponse>> GetAllCasesAsync();

        // Get one case by ID
        Task<CaseResponse?> GetCaseByIdAsync(int id);

        // Submit a brand new case
        Task<CaseResponse> CreateCaseAsync(CreateCaseRequest request);

        // Move a case to a new workflow status
        Task<CaseResponse?> UpdateCaseStatusAsync(int id, CaseStatus status, string? rejectionReason = null);

        // Run validation rules against a case
        Task<(bool IsValid, string Message)> ValidateCaseAsync(int id);

        // Delete a case
        Task<bool> DeleteCaseAsync(int id);
    }
}

