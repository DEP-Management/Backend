using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.Interfaces;
using DEP.Service.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using FileModel = DEP.Repository.Models.File;  // Uses alias here becourse of ambigurity between DEP.Repository.Models.File and System.IO.File

namespace DEP.Test.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IFileRepository> _repoMock = new();
        private readonly Mock<IEncryptionService> _encryptionMock = new();
        private readonly FileService _service;

        public FileServiceTests()
        {
            _service = new FileService(_repoMock.Object, _encryptionMock.Object);
        }

        public async Task AddFile_ShouldUploadAndReturnFile()
        {
            // Arrange
            var formFile = new Mock<IFormFile>().Object;
            var uploadedFile = new FileModel { FileName = "test.pdf" };
            _repoMock.Setup(r => r.UploadFile(formFile)).ReturnsAsync(uploadedFile);
            _repoMock.Setup(r => r.AddFile(It.IsAny<FileModel>())).ReturnsAsync((FileModel f) => f);

            // Act
            var result = await _service.AddFile(formFile, personId: 1, tagId: 2);

            // Assert
            Assert.Equal("test.pdf", result.FileName);
            Assert.Equal(1, result.PersonId);
            Assert.Equal(2, result.FileTagId);
            Assert.True(result.UploadDate <= DateTime.Now);
        }


        [Fact]
        public async Task AddMultipleFiles_ShouldReturnFileList()
        {
            // Arrange
            var formFiles = new List<IFormFile> { new Mock<IFormFile>().Object };
            var tags = new List<FileTag> { new FileTag { FileTagId = 1, TagName = "Test" } };
            var returnedFiles = new List<FileModel> { new FileModel { FileId = 1, FileName = "file1" } };

            _repoMock.Setup(r => r.UploadMultipleFiles(formFiles, tags, 1)).ReturnsAsync(returnedFiles);

            // Act
            var result = await _service.AddMultipleFiles(formFiles, tags, 1);

            // Assert
            Assert.Single(result);
            Assert.Equal("file1", result[0].FileName);
        }

        [Fact]
        public async Task DeleteFile_ShouldReturnDeletedFile()
        {
            // Arrange
            var file = new FileModel { FileId = 1 };
            _repoMock.Setup(r => r.DeleteFile(1)).ReturnsAsync(file);

            // Act
            var result = await _service.DeleteFile(1);

            // Assert
            Assert.Equal(1, result.FileId);
        }

        [Fact]
        public async Task GetFiles_ShouldReturnFileList()
        {
            // Arrange
            var files = new List<FileModel> { new FileModel { FileId = 1 } };
            _repoMock.Setup(r => r.GetFiles(1)).ReturnsAsync(files);

            // Act
            var result = await _service.GetFiles(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].FileId);
        }

        [Fact]
        public async Task GetFileById_ShouldReturnFile()
        {
            var file = new FileModel { FileId = 5 };
            _repoMock.Setup(r => r.GetFileById(5)).ReturnsAsync(file);

            var result = await _service.GetFileById(5);

            Assert.Equal(5, result.FileId);
        }

        [Fact]
        public async Task GetFileByName_ShouldReturnMatchingFiles()
        {
            var files = new List<FileModel>
        {
            new FileModel { FileName = "match1" },
            new FileModel { FileName = "match2" }
        };

            _repoMock.Setup(r => r.GetFileByName("match")).ReturnsAsync(files);

            var result = await _service.GetFileByName("match");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task UpdateFile_ShouldReturnUpdatedFile()
        {
            var file = new FileModel { FileId = 99, FileName = "updated" };
            _repoMock.Setup(r => r.UpdateFile(file)).ReturnsAsync(file);

            var result = await _service.UpdateFile(file);

            Assert.Equal(99, result.FileId);
            Assert.Equal("updated", result.FileName);
        }
    }
}
