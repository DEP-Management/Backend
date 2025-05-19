using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEP.Test.Services
{
    public class ModuleServiceTests
    {
        private readonly Mock<IModuleRepository> mockModuleRepo = new();
        private readonly Mock<ICourseRepository> mockCourseRepo = new();

        private readonly ModuleService service;

        public ModuleServiceTests()
        {
            service = new ModuleService(mockModuleRepo.Object, mockCourseRepo.Object);
        }

        [Fact]
        public async Task AddModule_Should_CallRepositoryAndReturnTrue()
        {
            // Arrange
            var module = new Module { ModuleId = 1, Name = "Test" };
            mockModuleRepo.Setup(r => r.AddModule(module)).ReturnsAsync(true);

            // Act
            var result = await service.AddModule(module);

            // Assert
            Assert.True(result);
            mockModuleRepo.Verify(r => r.AddModule(module), Times.Once);
        }

        [Fact]
        public async Task DeleteModule_Should_CallRepositoryAndReturnTrue()
        {
            // Arrange
            var id = 42;
            mockModuleRepo.Setup(r => r.DeleteModule(id)).ReturnsAsync(true);

            // Act
            var result = await service.DeleteModule(id);

            // Assert
            Assert.True(result);
            mockModuleRepo.Verify(r => r.DeleteModule(id), Times.Once);
        }

        [Fact]
        public async Task GetModules_Should_ReturnListOfModules()
        {
            // Arrange
            var modules = new List<Module> { new Module { ModuleId = 1, Name = "Test" } };
            mockModuleRepo.Setup(r => r.GetModules()).ReturnsAsync(modules);

            // Act
            var result = await service.GetModules();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test", result[0].Name);
        }

        [Fact]
        public async Task GetModulesWithCourse_Should_ReturnModulesWithCourses()
        {
            // Arrange
            var modules = new List<Module> { new Module { ModuleId = 1, Name = "Test", Description = "Desc" } };
            var courses = new List<Course> { new Course { CourseId = 10, ModuleId = 1 } };

            mockModuleRepo.Setup(r => r.GetModules()).ReturnsAsync(modules);
            mockCourseRepo.Setup(r => r.GetCourseWithPerson(1)).ReturnsAsync(courses);

            // Act
            var result = await service.GetModulesWithCourse();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].ModuleId);
            Assert.Equal("Test", result[0].Name);
            Assert.Single(result[0].Courses);
        }

        [Fact]
        public async Task GetModuleWithCourseInList_Should_ReturnSingleModuleWithCourses()
        {
            // Arrange
            var module = new Module { ModuleId = 2, Name = "Solo", Description = "Only one" };
            var courses = new List<Course> { new Course { CourseId = 20, ModuleId = 2 } };

            mockModuleRepo.Setup(r => r.GetModuleById(2)).ReturnsAsync(module);
            mockCourseRepo.Setup(r => r.GetCourseWithPerson(2)).ReturnsAsync(courses);

            // Act
            var result = await service.GetModuleWithCourseInList(2);

            // Assert
            Assert.Single(result);
            Assert.Equal("Solo", result[0].Name);
            Assert.Single(result[0].Courses);
        }

        [Fact]
        public async Task UpdateModule_Should_CallRepositoryAndReturnTrue()
        {
            // Arrange
            var module = new Module { ModuleId = 3, Name = "Update" };
            mockModuleRepo.Setup(r => r.UpdateModule(module)).ReturnsAsync(true);

            // Act
            var result = await service.UpdateModule(module);

            // Assert
            Assert.True(result);
            mockModuleRepo.Verify(r => r.UpdateModule(module), Times.Once);
        }
    }
}
