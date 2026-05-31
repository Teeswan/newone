using EPMS.Application.Interfaces;
using EPMS.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeKpisController : ControllerBase
    {
        private readonly IEmployeeKpiService _service;

        public EmployeeKpisController(IEmployeeKpiService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployeeId(int employeeId)
        {
            var result = await _service.GetByEmployeeIdAsync(employeeId);
            return Ok(result);
        }

        [HttpPost("calculate/{employeeId}")]
        public async Task<IActionResult> CalculateForEmployee(int employeeId)
        {
            try
            {
                var result = await _service.CalculateForEmployeeAsync(employeeId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeKpiRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.EmployeeKpiId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(BulkEmployeeKpiRequest request)
        {
            var result = await _service.CreateBulkAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EmployeeKpiRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
