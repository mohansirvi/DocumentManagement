using DocumentManagement.Models;
using DocumentManagement.Models.DTO;

namespace DocumentManagement.Services.IService
{
    public interface IDocumentService
    {
        Task<List<Document>> GetAllAsync(int pageNumber, int pageSize);
        Task<Document> GetByIdAsync(int id);
        Task<Document> CreateAsync(DocumentDto dto);
        Task<Document> UpdateAsync(int id, DocumentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
