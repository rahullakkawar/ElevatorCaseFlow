using ElevatorCaseFlow.Application.Interfaces;
using ElevatorCaseFlow.Domain.Entities;
using ElevatorCaseFlow.Domain.Enums;
using ElevatorCaseFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElevatorCaseFlow.Infrastructure.Repositories
{
    /// <summary>
    /// Handles all direct database operations for ElevatorCases.
    /// This is the ONLY place in the app that talks to SQL Server.
    /// Implements ICaseRepository contract defined in Application layer.
    /// </summary>
    public class CaseRepository : ICaseRepository
    {
        // DbContext is injected — repository never creates its own connection
        private readonly AppDbContext _context;

        public CaseRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches all elevator cases from the database.
        /// OrderByDescending shows newest cases first.
        /// </summary>
        public async Task<IEnumerable<ElevatorCase>> GetAllAsync()
        {
            return await _context.ElevatorCases
                .OrderByDescending(c => c.CreatedAt) // newest first
                .ToListAsync();
        }

        /// <summary>
        /// Fetches a single case by its numeric ID.
        /// Returns null if not found — caller must handle this.
        /// </summary>
        public async Task<ElevatorCase?> GetByIdAsync(int id)
        {
            return await _context.ElevatorCases
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Fetches a single case by its case number (e.g. CASE-20240101120000).
        /// Returns null if not found.
        /// </summary>
        public async Task<ElevatorCase?> GetByCaseNumberAsync(string caseNumber)
        {
            return await _context.ElevatorCases
                .FirstOrDefaultAsync(c => c.CaseNumber == caseNumber);
        }

        /// <summary>
        /// Saves a new elevator case to the database.
        /// EF Core automatically sets the Id after saving.
        /// </summary>
        public async Task<ElevatorCase> CreateAsync(ElevatorCase elevatorCase)
        {
            // Add the new case to the context (not saved yet)
            await _context.ElevatorCases.AddAsync(elevatorCase);

            // Actually save to SQL Server
            await _context.SaveChangesAsync();

            // Return the saved case — now has its database Id populated
            return elevatorCase;
        }

        /// <summary>
        /// Updates the workflow status of an existing case.
        /// Also records rejection reason if the case is being rejected.
        /// Returns null if case not found.
        /// </summary>
        public async Task<ElevatorCase?> UpdateStatusAsync(
            int id, CaseStatus status, string? rejectionReason = null)
        {
            var elevatorCase = await _context.ElevatorCases
                .FirstOrDefaultAsync(c => c.Id == id);

            // Return null if case doesn't exist
            if (elevatorCase == null) return null;

            // Update the status
            elevatorCase.Status = status;

            // Record rejection reason only if case is being rejected
            if (status == CaseStatus.Rejected)
                elevatorCase.RejectionReason = rejectionReason;

            // Stamp the update time
            elevatorCase.UpdatedAt = DateTime.UtcNow;

            // Save changes to SQL Server
            await _context.SaveChangesAsync();

            return elevatorCase;
        }

        /// <summary>
        /// Deletes a case from the database by ID.
        /// Returns true if deleted, false if case was not found.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var elevatorCase = await _context.ElevatorCases
                .FirstOrDefaultAsync(c => c.Id == id);

            // Nothing to delete
            if (elevatorCase == null) return false;

            // Remove from context and save
            _context.ElevatorCases.Remove(elevatorCase);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}


