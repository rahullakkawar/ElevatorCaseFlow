using ElevatorCaseFlow.Domain.Enums;
using System.Diagnostics;

namespace ElevatorCaseFlow.Domain.Entities
{
    public class ElevatorCase
    {
        public int Id { get; set; }

        public string CaseNumber { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string BuildingType { get; set; } = string.Empty;

        public int FloorCount { get; set; }

        public CaseStatus Status { get; set; } = CaseStatus.Submitted;

        public string XmlPayload { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}


