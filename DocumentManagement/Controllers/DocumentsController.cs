using DocumentManagement.Models.DTO;
using DocumentManagement.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.Controllers
{
    [Route("api/document")]
    [ApiController]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Page number and page size must be greater than zero.");
            }

            var documents = await _documentService.GetAllAsync(pageNumber, pageSize);
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _documentService.GetByIdAsync(id);
            return doc == null ? NotFound() : Ok(doc);
        }

        [HttpPost]
        [Authorize(Roles = "admin,editor")]
        public async Task<IActionResult> Create([FromBody] DocumentDto dto)
        {
            var result = await _documentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,editor")]
        public async Task<IActionResult> Update(int id, [FromBody] DocumentDto dto)
        {
            var result = await _documentService.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _documentService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}

