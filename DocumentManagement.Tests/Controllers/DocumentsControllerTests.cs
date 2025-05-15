using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DocumentManagement.Controllers;
using DocumentManagement.Services.IService;
using DocumentManagement.Models.DTO;
using DocumentManagement.Models;
namespace DocumentManagement.Tests.Controllers
{
    public class DocumentsControllerTests
    {
        private readonly Mock<IDocumentService> _mockService;
        private readonly DocumentsController _controller;

        public DocumentsControllerTests()
        {
            _mockService = new Mock<IDocumentService>();
            _controller = new DocumentsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithDocuments()
        {
            // Arrange  
            var documents = new List<Document>
            {
               new Document { Title = "Doc1", Content = "content", UserId = 1 },
               new Document { Title = "Doc2", Content = "content", UserId = 2 }
            };
            _mockService.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(documents.ToList());

            // Act  
            var result = await _controller.GetAll(1, 10);

            // Assert  
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDocs = okResult.Value.Should().BeAssignableTo<IEnumerable<Document>>().Subject;
            returnedDocs.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsDocument()
        {
            // Arrange  
            var document = new Document { Id = 1, Title = "Doc1", Content = "content", UserId = 1 };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(document);

            // Act  
            var result = await _controller.GetById(1);

            // Assert  
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDoc = okResult.Value.Should().BeOfType<Document>().Subject;
            returnedDoc.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            // Arrange  
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Document)null);

            // Act  
            var result = await _controller.GetById(99);

            // Assert  
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ValidDocument_ReturnsCreatedAtAction()
        {
            // Arrange  
            var document = new DocumentDto { Title = "Doc1", Content = "content", UserId = 1 };
            var createdDocument = new Document { Id = 10, Title = "New Doc", Content = "content", UserId = 1 };
            _mockService.Setup(s => s.CreateAsync(It.IsAny<DocumentDto>())).ReturnsAsync(createdDocument);

            // Act  
            var result = await _controller.Create(document);

            // Assert  
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.RouteValues["id"].Should().Be(10);
            ((Document)createdResult.Value).Title.Should().Be("New Doc");
        }

        [Fact]
        public async Task Update_ExistingDocument_ReturnsOk()
        {
            // Arrange  
            var updated = new DocumentDto { Title = "Updated Doc", Content = "content", UserId = 1 };
            var updatedDocument = new Document { Id = 1, Title = "Updated Doc", Content = "content", UserId = 1 };
            _mockService.Setup(s => s.UpdateAsync(1, It.IsAny<DocumentDto>())).ReturnsAsync(updatedDocument);

            // Act  
            var result = await _controller.Update(1, updated);

            // Assert  
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            ((Document)okResult.Value).Title.Should().Be("Updated Doc");
        }

        [Fact]
        public async Task Update_NonExistingDocument_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.UpdateAsync(99, It.IsAny<DocumentDto>())).ReturnsAsync((Document)null);

            // Act
            var result = await _controller.Update(99, new DocumentDto());

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ExistingDocument_ReturnsNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_NonExistingDocument_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
