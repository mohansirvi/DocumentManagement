using DocumentManagement.Data;
using DocumentManagement.Enums;
using DocumentManagement.Models.DTO;
using DocumentManagement.Models;
using Microsoft.EntityFrameworkCore;
using DocumentManagement.Services.IService;

namespace DocumentManagement.Services
{
    public class IngestionService : IIngestionService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<IngestionService> _logger;
        private readonly string _baseUrl;

        public IngestionService(ApplicationDbContext context, HttpClient httpClient, IConfiguration config, ILogger<IngestionService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var baseUrl = config["IngestionService:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("BaseUrl configuration is missing or empty.", nameof(config));
            }
            _baseUrl = baseUrl;
        }

        public async Task<List<IngestionRequest>> GetAllAsync()
        {
            return await _context.IngestionRequests
                .Include(i => i.Document)
                .ToListAsync();
        }

        public async Task<IngestionRequest> TriggerIngestionAsync(IngestionRequestDto dto)
        {
            await ValidateIngestionRequestDto(dto);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ingestion = new IngestionRequest
                {
                    DocumentId = dto.DocumentId,
                    Status = IngestionStatus.Pending.ToString(),
                    RequestedAt = DateTime.UtcNow
                };

                _context.IngestionRequests.Add(ingestion);
                await _context.SaveChangesAsync();

                //var payload = JsonSerializer.Serialize(new { documentId = dto.DocumentId });
                //var content = new StringContent(payload, Encoding.UTF8, "application/json");

                //var response = await _httpClient.PostAsync($"{_baseUrl}/ingest", content);
                //response.EnsureSuccessStatusCode();

                ingestion.Status = IngestionStatus.InProgress.ToString();
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Ingestion for DocumentId: {DocumentId} is now InProgress", dto.DocumentId);
                return ingestion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to trigger ingestion for DocumentId: {DocumentId}", dto.DocumentId);
                throw;
            }
        }

        public async Task<IngestionRequest> UpdateStatusAsync(int id, IngestionStatus status)
        {
            var ingestion = await _context.IngestionRequests.FindAsync(id);
            if (ingestion == null) return null;

            ingestion.Status = status.ToString();
            await _context.SaveChangesAsync();
            return ingestion;
        }

        private async Task ValidateIngestionRequestDto(IngestionRequestDto dto)
        {
            if (dto.DocumentId <= 0)
                throw new ArgumentException("DocumentId must be greater than zero.");

            var documentExists = await _context.Documents.AnyAsync(d => d.Id == dto.DocumentId);
            if (!documentExists)
                throw new ArgumentException($"Document with ID {dto.DocumentId} does not exist.");
        }
    }
}