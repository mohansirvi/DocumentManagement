using DocumentManagement.Enums;
using DocumentManagement.Models.DTO;
using DocumentManagement.Models;

namespace DocumentManagement.Services.IService
{
    public interface IIngestionService
    {
        Task<List<IngestionRequest>> GetAllAsync();
        Task<IngestionRequest> TriggerIngestionAsync(IngestionRequestDto dto);
        Task<IngestionRequest> UpdateStatusAsync(int id, IngestionStatus status);
    }
}
