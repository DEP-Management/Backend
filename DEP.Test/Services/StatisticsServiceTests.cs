using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.Interfaces;
using DEP.Service.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEP.Test.Services
{
    public class StatisticsServiceTests
    {
        private readonly Mock<IPersonRepository> mockPersonRepo = new();
        private readonly Mock<IDepartmentRepository> mockDepartmentRepo = new();
        private readonly Mock<ILocationRepository> mockLocationRepo = new();
        private readonly Mock<IPersonCourseRepository> mockPersonCourseRepo = new();
        private readonly Mock<IEncryptionService> mockEncryption = new();
        private readonly Mock<ICourseRepository> mockCourseRepo = new();
        private readonly Mock<IModuleRepository> mockModuleRepo = new();

        private readonly StatisticsService service;

        public StatisticsServiceTests()
        {
            service = new StatisticsService(
                mockPersonRepo.Object,
                mockDepartmentRepo.Object,
                mockLocationRepo.Object,
                mockPersonCourseRepo.Object,
                mockEncryption.Object,
                mockCourseRepo.Object,
                mockModuleRepo.Object
            );
        }

        [Fact]
        public async Task GetPersonsPerDepartment_Should_ReturnCorrectCounts()
        {
            // Arrange
            var departments = new List<Department>
        {
            new Department { DepartmentId = 1, Name = "Pædagogik" },
            new Department { DepartmentId = 2, Name = "Ledelse" }
        };

            var persons = new List<Person>
        {
            new Person { PersonId = 1, DepartmentId = 1 },
            new Person { PersonId = 2, DepartmentId = 1 },
            new Person { PersonId = 3, DepartmentId = 2 }
        };

            mockDepartmentRepo.Setup(r => r.GetDepartments()).ReturnsAsync(departments);
            mockPersonRepo.Setup(r => r.GetPersons()).ReturnsAsync(persons);

            // Act
            var result = await service.GetPersonsPerDepartment();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.DepartmentId == 1 && r.TeacherCount == 2);
            Assert.Contains(result, r => r.DepartmentId == 2 && r.TeacherCount == 1);
        }

        [Fact]
        public async Task GetCourseStatusCountByModule_Should_CountStatusesCorrectly()
        {
            // Arrange
            var moduleId = 5;
            var courses = new List<PersonCourse>
    {
        new PersonCourse { PersonId = 1, Status = Status.Begyndt },
        new PersonCourse { PersonId = 2, Status = Status.Ikke_gennemført },
        new PersonCourse { PersonId = 3, Status = Status.Begyndt }
    };

            mockPersonCourseRepo.Setup(r => r.GetPersonCoursesByModule(moduleId)).ReturnsAsync(courses);

            // Act
            var result = await service.GetCourseStatusCountByModule(moduleId);

            // Assert
            Assert.Contains(result, r => r.StatusId == (int)Status.Begyndt && r.PersonCount == 2);
            Assert.Contains(result, r => r.StatusId == (int)Status.Ikke_gennemført && r.PersonCount == 1);
        }

    }
}
