using DocumentManagement.Models.DTO;
using DocumentManagement.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.Controllers
{
    [Route("api/ingestion")]
    [ApiController]
    [Authorize(Roles = "admin,editor")]
    public class IngestionController : ControllerBase
    {
        private readonly IIngestionService _ingestionService;

        public IngestionController(IIngestionService ingestionService)
        {
            _ingestionService = ingestionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingestions = await _ingestionService.GetAllAsync();
            return Ok(ingestions);
        }

        [HttpPost]
        public async Task<IActionResult> Trigger([FromBody] IngestionRequestDto dto)
        {
            var request = await _ingestionService.TriggerIngestionAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = request.Id }, request);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] IngestionStatusUpdateDto dto)
        {
            var updated = await _ingestionService.UpdateStatusAsync(id, dto.Status);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}

