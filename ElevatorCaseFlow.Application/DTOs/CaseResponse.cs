namespace ElevatorCaseFlow.Application.DTOs
{
    /// <summary>
    /// Data returned to the client after a case is created or retrieved.
    /// </summary>
    public class CaseResponse
    {
        // Unique database ID of the case
        public int Id { get; set; }

        // Auto-generated unique case reference number (e.g. CASE-00001)
        public string CaseNumber { get; set; } = string.Empty;

        // Name of the client who submitted the case
        public string ClientName { get; set; } = string.Empty;

        // Country of installation
        public string Country { get; set; } = string.Empty;

        // Type of building
        public string BuildingType { get; set; } = string.Empty;

        // Number of floors
        public int FloorCount { get; set; }

        // Current workflow status as a readable string (e.g. "Submitted", "Validated")
        public string Status { get; set; } = string.Empty;

        // Reason for rejection — only populated if status is Rejected
        public string? RejectionReason { get; set; }

        // When the case was first submitted
        public DateTime CreatedAt { get; set; }

        // When the case was last updated
        public DateTime? UpdatedAt { get; set; }
    }
}
