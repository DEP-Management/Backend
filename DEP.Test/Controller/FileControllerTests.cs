using DEP.Controllers;
using DEP.Repository.Context;
using DEP.Repository.Models;
using DEP.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;
using System.Text.Json;
using FileModel = DEP.Repository.Models.File;
using Microsoft.EntityFrameworkCore;


namespace DEP.Test.Controller
{
    public class FileControllerTests
    {
        private readonly Mock<IFileService> _mockService;
        private readonly Mock<DatabaseContext> _mockContext;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly FileController _controller;

        public FileControllerTests()
        {
            _mockService = new Mock<IFileService>();
            _mockContext = new Mock<DatabaseContext>();
            _mockConfig = new Mock<IConfiguration>();
            _controller = new FileController(_mockService.Object, _mockContext.Object, _mockConfig.Object);
        }

        [Fact]
        public async Task AddMultipleFiles_ReturnsOk_WhenFilesUploaded()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("test")), 0, 4, "Data", "test.txt")
            };

            var fileTags = JsonSerializer.Serialize(new List<FileTag> { new FileTag { TagName = "Tag1" } });
            int personId = 1;

            _mockService.Setup(s => s.AddMultipleFiles(It.IsAny<List<IFormFile>>(), It.IsAny<List<FileTag>>(), personId))
                        .ReturnsAsync(new List<FileModel>());

            // Act
            var result = await _controller.AddMultipleFiles(files, fileTags, personId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<FileModel>>(okResult.Value);
        }

        [Fact]
        public async Task AddMultipleFiles_ReturnsBadRequest_WhenNoFiles()
        {
            // Arrange
            var files = new List<IFormFile>();
            var fileTags = JsonSerializer.Serialize(new List<FileTag> { new FileTag { TagName = "Tag1" } });
            int personId = 1;

            // Act
            var result = await _controller.AddMultipleFiles(files, fileTags, personId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No files uploaded.", badRequest.Value);
        }
    }
}
