using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DocumentManagement.Controllers;
using DocumentManagement.Models.DTO;
using DocumentManagement.Services.IService;
using DocumentManagement.Enums;
using DocumentManagement.Models;

namespace DocumentManagement.Tests.Controllers
{
    public class IngestionControllerTests
    {
        private readonly Mock<IIngestionService> _mockService;
        private readonly IngestionController _controller;

        public IngestionControllerTests()
        {
            _mockService = new Mock<IIngestionService>();
            _controller = new IngestionController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfIngestionRequests()
        {
            // Arrange  
            var ingestions = new List<IngestionRequest>
            {
               new IngestionRequest { DocumentId = 1 },
               new IngestionRequest { DocumentId = 2 }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(ingestions);

            // Act  
            var result = await _controller.GetAll();

            // Assert  
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = ok.Value.Should().BeAssignableTo<IEnumerable<IngestionRequest>>().Subject;
            value.Should().HaveCount(2);
        }

        [Fact]
        public async Task Trigger_ValidRequest_ReturnsCreatedAt()
        {
            // Arrange  
            var dto = new IngestionRequestDto { DocumentId = 5 };
            var response = new IngestionRequest { DocumentId = 5 };

            _mockService.Setup(s => s.TriggerIngestionAsync(dto)).ReturnsAsync(response);

            // Act  
            var result = await _controller.Trigger(dto);

            // Assert  
            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            created.ActionName.Should().Be(nameof(_controller.GetAll));
            created.Value.Should().BeAssignableTo<IngestionRequest>(); 
        }

        [Fact]
        public async Task UpdateStatus_ExistingId_ReturnsOk()
        {
            // Arrange  
            var id = 1;
            var dto = new IngestionStatusUpdateDto { Status = IngestionStatus.InProgress };
            var updatedDto = new IngestionRequest { DocumentId = 1 }; 

            _mockService.Setup(s => s.UpdateStatusAsync(id, dto.Status)).ReturnsAsync(updatedDto);

            // Act  
            var result = await _controller.UpdateStatus(id, dto);

            // Assert  
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IngestionRequest>(); 
        }

        [Fact]
        public async Task UpdateStatus_NonExistingId_ReturnsNotFound()
        {
            // Arrange  
            var id = 999;
            var dto = new IngestionStatusUpdateDto { Status = IngestionStatus.Cancelled };

            _mockService.Setup(s => s.UpdateStatusAsync(id, dto.Status)).ReturnsAsync((IngestionRequest)null);

            // Act  
            var result = await _controller.UpdateStatus(id, dto);

            // Assert  
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
