using DocumentManagement.Data;
using DocumentManagement.Models;
using DocumentManagement.Models.DTO;
using DocumentManagement.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(ApplicationDbContext context, ILogger<DocumentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Document>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Documents
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Document> GetByIdAsync(int id)
        {
            var document = await _context.Documents.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (document == null) return null;
            return document;
        }

        public async Task<Document> CreateAsync(DocumentDto dto)
        {
            var document = new Document
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Document created with ID: {DocumentId}", document.Id);
            return document;
        }

        public async Task<Document> UpdateAsync(int id, DocumentDto dto)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null) return null;

            document.Title = dto.Title;
            document.Content = dto.Content;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Document with ID: {DocumentId} updated successfully", id);
            return document;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                _logger.LogWarning("Attempted to delete non-existent document with ID: {DocumentId}", id);
                return false;
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Document with ID: {DocumentId} deleted successfully", id);
            return true;
        }
    }
}
