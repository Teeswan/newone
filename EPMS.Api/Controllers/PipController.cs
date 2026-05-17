using EPMS.Application.Interfaces;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class PipController : ControllerBase
    {
        private readonly IPipService _pipService;

        public PipController(IPipService pipService) => _pipService = pipService;

        // ── PLANS ─────────────────────────────────────────────────────────────

        /// <summary>Get all PIP plans (HR / Admin view)</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _pipService.GetAllPipsAsync(ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Get a specific PIP with full details</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _pipService.GetPipByIdAsync(id, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get all PIPs for a specific employee</summary>
        [HttpGet("employee/{employeeId:int}")]
        public async Task<IActionResult> GetByEmployee(int employeeId, CancellationToken ct)
        {
            var result = await _pipService.GetPipsByEmployeeAsync(employeeId, ct);
            return Ok(result);
        }

        /// <summary>Get all PIPs managed by a specific manager</summary>
        [HttpGet("manager/{managerId:int}")]
        public async Task<IActionResult> GetByManager(int managerId, CancellationToken ct)
        {
            var result = await _pipService.GetPipsByManagerAsync(managerId, ct);
            return Ok(result);
        }

        /// <summary>Create a new PIP with initial objectives</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePipPlanRequest request, CancellationToken ct)
        {
            var result = await _pipService.CreatePipAsync(request, ct);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.PipId }, result);
        }

        /// <summary>Update PIP status, end date, or overall goal</summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePipPlanRequest request, CancellationToken ct)
        {
            var result = await _pipService.UpdatePipAsync(request, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Delete a PIP plan</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _pipService.DeletePipAsync(id, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // ── OBJECTIVES ────────────────────────────────────────────────────────

        /// <summary>Add a new objective to an existing PIP</summary>
        [HttpPost("objectives")]
        public async Task<IActionResult> AddObjective(
            [FromBody] CreatePipObjectiveRequest request, CancellationToken ct)
        {
            var result = await _pipService.AddObjectiveAsync(request, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Update objective achievement status and review comments</summary>
        [HttpPut("objectives")]
        public async Task<IActionResult> UpdateObjective(
            [FromBody] UpdatePipObjectiveRequest request, CancellationToken ct)
        {
            var result = await _pipService.UpdateObjectiveAsync(request, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Delete an objective</summary>
        [HttpDelete("objectives/{id:int}")]
        public async Task<IActionResult> DeleteObjective(int id, CancellationToken ct)
        {
            var result = await _pipService.DeleteObjectiveAsync(id, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // ── MEETINGS ──────────────────────────────────────────────────────────

        /// <summary>Schedule a follow-up review meeting</summary>
        [HttpPost("meetings")]
        public async Task<IActionResult> AddMeeting(
            [FromBody] CreatePipMeetingRequest request, CancellationToken ct)
        {
            var result = await _pipService.AddMeetingAsync(request, ct);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Update meeting details and progress notes</summary>
        [HttpPut("meetings")]
        public async Task<IActionResult> UpdateMeeting(
            [FromBody] UpdatePipMeetingRequest request, CancellationToken ct)
        {
            var result = await _pipService.UpdateMeetingAsync(request, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Delete a scheduled meeting</summary>
        [HttpDelete("meetings/{id:int}")]
        public async Task<IActionResult> DeleteMeeting(int id, CancellationToken ct)
        {
            var result = await _pipService.DeleteMeetingAsync(id, ct);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // ── REPORTS ───────────────────────────────────────────────────────────

        /// <summary>Get PIP summary report (powered by Dapper + Stored Procedure)</summary>
        [HttpGet("reports/summary")]
        public async Task<IActionResult> GetSummaryReport(
            [FromQuery] int? managerId,
            [FromQuery] string? status,
            CancellationToken ct)
        {
            var result = await _pipService.GetSummaryReportAsync(managerId, status, ct);
            return Ok(result);
        }
    }
}
