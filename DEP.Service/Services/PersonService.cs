using System;
using System.Reflection.Metadata.Ecma335;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Repository.Repositories;
using DEP.Service.EncryptionHelpers;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;
using DEP.Service.ViewModels.Statistic;

namespace DEP.Service.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository repo;
        private readonly IEncryptionService encryptionService;
        public PersonService(IPersonRepository repo, IEncryptionService encryptionService)
        {
            this.repo = repo;
            this.encryptionService = encryptionService;
        }

        public async Task<Person?> AddPerson(Person person)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            person.HiringDate = TimeZoneInfo.ConvertTimeFromUtc(person.HiringDate, localTimeZone);
            person.HiringDate = person.HiringDate.Date;
            person.EndDate = person.HiringDate.AddYears(4);

            PersonEncryptionHelper.Encrypt(person, encryptionService);
            return await repo.AddPerson(person);
        }

        public async Task<bool> DeletePerson(int id)
        {
            return await repo.DeletePerson(id);
        }

        public async Task<Person> GetPersonById(int personId)
        {
            var person = await repo.GetPersonById(personId);

            PersonEncryptionHelper.Decrypt(person, encryptionService);

            return person;
        }

        public async Task<List<Person>> GetPersons()
        {
            var persons = await repo.GetPersons();

            PersonEncryptionHelper.Decrypt(persons, encryptionService);

            return persons;
        }

        public async Task<List<Person>> GetPersonsExcel(int leaderId)
        {
            var persons = await repo.GetPersonsExcel(leaderId);

            PersonEncryptionHelper.Decrypt(persons, encryptionService);

            return persons;
        }
        
        public async Task<List<Person>> GetPersonExcel(int id)
        {
            var personList = new List<Person>();

            var person = await repo.GetPersonExcel(id);

            PersonEncryptionHelper.Decrypt(person, encryptionService);

            personList.Add(person);
            //personList.Add(await repo.GetPersonExcel(id));

            return personList;
        }


        public async Task<List<Person>> GetPersonsByCourseId(int courseId)
        {
            var persons = await repo.GetPersonsByCourseId(courseId);

            PersonEncryptionHelper.Decrypt(persons, encryptionService);

            return persons;
        }

        public async Task<List<Person>> GetPersonsByEducationalLeaderId(int leaderId)
        {
            return await repo.GetPersonsByCourseId(leaderId);
        }

        public async Task<List<Person>> GetPersonsByDepartmentAndLocation(int departmentId, int locationId)
        {
            return await repo.GetPersonsByDepartmentAndLocation(departmentId, locationId);
        }

        public async Task<List<Person>> GetPersonsNotInCourse(int courseId)
        {
            return await repo.GetPersonsNotInCourse(courseId);
        }

        public async Task<List<PersonToTabelsViewModel>> GetPersonsTabel()
        {
            var people = await repo.GetPersons();

            List<PersonToTabelsViewModel> NewPeople = new List<PersonToTabelsViewModel>();

            foreach (var item in people)
            {
                PersonToTabelsViewModel peps = new PersonToTabelsViewModel()
                {
                    Name = item.Name,
                    Initials = item.Initials,
                    HiringDate = item.HiringDate,
                    EndDate = item.EndDate,
                    SvuEligible = item.SvuEligible,
                    EducationalConsultant = item.EducationalConsultant,
                    OperationCoordinator = item.OperationCoordinator,
                    Department = item.Department,
                    Location = item.Location
                };

                NewPeople.Add(peps);
            }

            return NewPeople;
        }

        public async Task<bool> UpdatePerson(Person person)
        {
            PersonEncryptionHelper.Encrypt(person, encryptionService);
            return await repo.UpdatePerson(person);
        }
    }
}
