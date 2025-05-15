using DocumentManagement.Models;
using DocumentManagement.Models.DTO;
using DocumentManagement.Services;
using DocumentManagement.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;

namespace DocumentManagement.Tests.Service
{
    public class DocumentServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DocumentService _service;
        private readonly Mock<ILogger<DocumentService>> _loggerMock;

        public DocumentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DocumentDbTest")
                .Options;
            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<DocumentService>>();
            _service = new DocumentService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddDocument()
        {
            var dto = new DocumentDto { Title = "Doc 1", Content = "Content", UserId = 1 };

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Doc 1", result.Title);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedDocuments()
        {
            _context.Documents.AddRange(new Document { Title = "A", Content = "C1", UserId = 1 },
                                        new Document { Title = "B", Content = "C2", UserId = 1 });
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsync(1, 1);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectDocument()
        {
            var doc = new Document { Title = "Test", Content = "TestContent", UserId = 1 };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(doc.Id);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyDocument()
        {
            var doc = new Document { Title = "Old", Content = "Old", UserId = 1 };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            var dto = new DocumentDto { Title = "New", Content = "New", UserId = 1 };
            var updated = await _service.UpdateAsync(doc.Id, dto);

            Assert.NotNull(updated);
            Assert.Equal("New", updated.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDocument()
        {
            var doc = new Document { Title = "ToDelete", Content = "Content", UserId = 1 };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(doc.Id);

            Assert.True(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
