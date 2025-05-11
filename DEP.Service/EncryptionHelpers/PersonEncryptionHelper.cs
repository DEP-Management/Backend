using DEP.Repository.Models;
using DEP.Service.Interfaces;

namespace DEP.Service.EncryptionHelpers
{
    public static class PersonEncryptionHelper
    {
        public static void Encrypt(Person person, IEncryptionService encryptionService)
        {
            person.Name = encryptionService.Encrypt(person.Name);
            person.Initials = encryptionService.Encrypt(person.Initials);
        }

        // Basic overload for single person (for GetPersonById etc.)
        public static void Decrypt(Person person, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();

            Decrypt(person, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers);
        }

        // For lists
        public static void Decrypt(List<Person> people, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();

            foreach (var person in people)
            {
                Decrypt(person, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers);
            }
        }

        // Internal shared decrypt method
        private static void Decrypt(
            Person person,
            IEncryptionService encryptionService,
            HashSet<Department> decryptedDepartments,
            HashSet<Location> decryptedLocations,
            HashSet<User> decryptedUsers)
        {
            person.Name = encryptionService.Decrypt(person.Name);
            person.Initials = encryptionService.Decrypt(person.Initials);

            if (person.Department != null && !decryptedDepartments.Contains(person.Department))
            {
                DepartmentEncryptionHelper.Decrypt(person.Department, encryptionService);
                decryptedDepartments.Add(person.Department);
            }

            if (person.Location != null && !decryptedLocations.Contains(person.Location))
            {
                LocationEncryptionHelper.Decrypt(person.Location, encryptionService);
                decryptedLocations.Add(person.Location);
            }

            if (person.OperationCoordinator != null && !decryptedUsers.Contains(person.OperationCoordinator))
            {
                UserEncryptionHelper.DecryptUpdatableFields(person.OperationCoordinator, encryptionService);
                decryptedUsers.Add(person.OperationCoordinator);
            }

            if (person.EducationalConsultant != null && !decryptedUsers.Contains(person.EducationalConsultant))
            {
                UserEncryptionHelper.DecryptUpdatableFields(person.EducationalConsultant, encryptionService);
                decryptedUsers.Add(person.EducationalConsultant);
            }

            if (person.EducationalLeader != null && !decryptedUsers.Contains(person.EducationalLeader))
            {
                UserEncryptionHelper.DecryptUpdatableFields(person.EducationalLeader, encryptionService);
                decryptedUsers.Add(person.EducationalLeader);
            }
        }
    }
}
