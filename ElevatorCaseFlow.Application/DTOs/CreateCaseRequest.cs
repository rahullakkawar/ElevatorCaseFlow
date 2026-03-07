namespace ElevatorCaseFlow.Application.DTOs
{
    /// <summary>
    /// Data sent by the client when submitting a new elevator case request.
    /// </summary>
    public class CreateCaseRequest
    {
        // Name of the client submitting the case (e.g. KONE North America)
        public string ClientName { get; set; } = string.Empty;

        // Country where the elevator will be installed
        public string Country { get; set; } = string.Empty;

        // Type of building (Office, Residential, Commercial, Industrial)
        public string BuildingType { get; set; } = string.Empty;

        // Number of floors the elevator needs to serve
        public int FloorCount { get; set; }

        // Raw XML payload containing the full elevator design specifications
        public string XmlPayload { get; set; } = string.Empty;
    }
}


