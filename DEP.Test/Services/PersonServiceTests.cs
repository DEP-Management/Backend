using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.EncryptionHelpers;
using DEP.Service.Interfaces;
using DEP.Service.Services;
using Moq;
using Xunit;

namespace DEP.Test.Services
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> mockRepo;
        private readonly Mock<IEncryptionService> mockEncryption;
        private readonly PersonService service;

        public PersonServiceTests()
        {
            mockRepo = new Mock<IPersonRepository>();
            mockEncryption = new Mock<IEncryptionService>();
            service = new PersonService(mockRepo.Object, mockEncryption.Object);
        }

        [Fact]
        public async Task AddPerson_Should_SetEndDateCorrectly()
        {
            // Arrange
            var inputPerson = new Person
            {
                Name = "Test Person",
                HiringDate = new DateTime(2020, 1, 1)
            };

            mockRepo.Setup(x => x.AddPerson(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            // Act
            var result = await service.AddPerson(inputPerson);

            // Assert
            mockRepo.Verify(x => x.AddPerson(It.IsAny<Person>()), Times.Once);
            Assert.Equal(new DateTime(2024, 1, 1), result.EndDate); // 2020 + 4 år
        }


        [Fact]
        public async Task GetPersonById_Should_ReturnDecryptedPerson()
        {
            // Arrange
            var person = new Person { PersonId = 1, Name = "Test Person" };

            mockRepo.Setup(x => x.GetPersonById(1)).ReturnsAsync(person);

            mockEncryption.Setup(e => e.Decrypt(It.IsAny<string>())).Returns<string>(s => s);

            // Act
            var result = await service.GetPersonById(1);

            // Assert
            Assert.Equal("Test Person", result.Name);
        }

        [Fact]
        public async Task GetPersons_Should_DecryptPersons()
        {
            var persons = new List<Person>
    {
        new Person { PersonId = 1, Name = "Test1", Initials = ""},
        new Person { PersonId = 2, Name = "Test2", Initials = ""}
    };

            mockRepo.Setup(r => r.GetPersons()).ReturnsAsync(persons);
            mockEncryption.Setup(e => e.Decrypt(It.IsAny<string>())).Returns<string>(s => s);

            var result = await service.GetPersons();

            mockEncryption.Verify(e => e.Decrypt(It.IsAny<string>()), Times.AtLeastOnce); // mere robust
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task UpdatePerson_Should_EncryptBeforeUpdate()
        {
            // Arrange
            var person = new Person
            { PersonId = 1, Name = "Updated Person", Initials = "" };

            mockRepo.Setup(r => r.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(true);
            mockEncryption.Setup(e => e.Encrypt(It.IsAny<string>())).Returns<string>(s => s); // Simpel mock

            // Act
            var result = await service.UpdatePerson(person);

            // Assert
            mockEncryption.Verify(e => e.Encrypt(It.IsAny<string>()), Times.AtLeastOnce); // eller Times.Exactly(3)
            Assert.True(result);
        }

        [Fact]
        public async Task DeletePerson_Should_CallRepository()
        {
            mockRepo.Setup(r => r.DeletePerson(1)).ReturnsAsync(true);

            var result = await service.DeletePerson(1);

            Assert.True(result);
            mockRepo.Verify(r => r.DeletePerson(1), Times.Once);
        }
    }
}
