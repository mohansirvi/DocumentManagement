using DocumentManagement.Data;
using DocumentManagement.Enums;
using DocumentManagement.Models.DTO;
using DocumentManagement.Models;
using DocumentManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DocumentManagement.Tests.Service
{
    public class IngestionServiceTests
    {
        private readonly IngestionService _service;
        private readonly ApplicationDbContext _context;
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<IngestionService>> _loggerMock;
        private readonly IConfiguration _config;

        public IngestionServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("IngestionDbTest")
                .Options;
            _context = new ApplicationDbContext(options);

            _httpHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpHandlerMock.Object);
            _loggerMock = new Mock<ILogger<IngestionService>>();

            var inMemorySettings = new Dictionary<string, string> {
            {
                "IngestionService:BaseUrl", "http://fakeapi.com"}
            };
            _config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            _service = new IngestionService(_context, _httpClient, _config, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllIngestions()
        {
            var doc = new Document { Title = "doc", Content = "test", UserId = 1 };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            _context.IngestionRequests.Add(new IngestionRequest { DocumentId = doc.Id, Status = "Pending", RequestedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task TriggerIngestionAsync_ShouldThrow_WhenDocumentDoesNotExist()
        {
            var dto = new IngestionRequestDto { DocumentId = 999 };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.TriggerIngestionAsync(dto));
            Assert.Contains("does not exist", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenValidId()
        {
            var doc = new Document { Title = "doc", Content = "test", UserId = 1 };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            var ingestion = new IngestionRequest { DocumentId = doc.Id, Status = "Pending", RequestedAt = DateTime.UtcNow };
            _context.IngestionRequests.Add(ingestion);
            await _context.SaveChangesAsync();

            var result = await _service.UpdateStatusAsync(ingestion.Id, IngestionStatus.Completed);

            Assert.NotNull(result);
            Assert.Equal(IngestionStatus.Completed.ToString(), result.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnNull_WhenInvalidId()
        {
            var result = await _service.UpdateStatusAsync(9999, IngestionStatus.Completed);
            Assert.Null(result);
        }
    }
}
