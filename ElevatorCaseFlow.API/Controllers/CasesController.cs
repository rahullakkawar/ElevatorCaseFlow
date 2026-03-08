using ElevatorCaseFlow.Application.DTOs;
using ElevatorCaseFlow.Application.Interfaces;
using ElevatorCaseFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElevatorCaseFlow.API.Controllers
{
    /// <summary>
    /// Handles all HTTP requests related to elevator case management.
    /// Base route: /api/cases
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require JWT token by default
    public class CasesController : ControllerBase
    {
        private readonly ICaseService _caseService;

        // CaseService is injected automatically by DI container
        public CasesController(ICaseService caseService)
        {
            _caseService = caseService;
        }

        /// <summary>
        /// GET /api/cases
        /// Returns all elevator cases. Newest first.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cases = await _caseService.GetAllCasesAsync();
            return Ok(cases); // 200 OK with list of cases
        }

        /// <summary>
        /// GET /api/cases/{id}
        /// Returns a single case by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _caseService.GetCaseByIdAsync(id);

            // Return 404 if case not found
            if (result == null)
                return NotFound(new { Message = $"Case with ID {id} not found." });

            return Ok(result); // 200 OK with case data
        }

        /// <summary>
        /// POST /api/cases
        /// Submit a new elevator case request.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCaseRequest request)
        {
            // ModelState checks basic validation automatically
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 Bad Request

            var created = await _caseService.CreateCaseAsync(request);

            // 201 Created — includes the URL where the new case can be found
            return CreatedAtAction(nameof(GetById),
                new { id = created.Id }, created);
        }

        /// <summary>
        /// PUT /api/cases/{id}/status
        /// Update the workflow status of a case.
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] UpdateStatusRequest request)
        {
            // Parse the status string to enum value
            if (!Enum.TryParse<CaseStatus>(request.Status, true, out var status))
                return BadRequest(new { Message = "Invalid status value." });

            var updated = await _caseService.UpdateCaseStatusAsync(
                id, status, request.RejectionReason);

            if (updated == null)
                return NotFound(new { Message = $"Case with ID {id} not found." });

            return Ok(updated); // 200 OK with updated case
        }

        /// <summary>
        /// POST /api/cases/{id}/validate
        /// Run business validation rules against a submitted case.
        /// Automatically moves case to Validated or Rejected status.
        /// </summary>
        [HttpPost("{id}/validate")]
        public async Task<IActionResult> Validate(int id)
        {
            var (isValid, message) = await _caseService.ValidateCaseAsync(id);

            // Return result with appropriate HTTP status
            return Ok(new
            {
                IsValid = isValid,
                Message = message
            });
        }

        /// <summary>
        /// DELETE /api/cases/{id}
        /// Delete a case by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _caseService.DeleteCaseAsync(id);

            if (!deleted)
                return NotFound(new { Message = $"Case with ID {id} not found." });

            // 204 No Content — standard response for successful delete
            return NoContent();
        }
    }

    /// <summary>
    /// Request body for updating a case status.
    /// </summary>
    public class UpdateStatusRequest
    {
        // Status as string (e.g. "Validated", "Processing", "Completed", "Rejected")
        public string Status { get; set; } = string.Empty;

        // Optional — only needed when rejecting a case
        public string? RejectionReason { get; set; }
    }
}

