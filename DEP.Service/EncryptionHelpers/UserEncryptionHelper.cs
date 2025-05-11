using DEP.Repository.Models;
using DEP.Repository.ViewModels;
using DEP.Service.Interfaces;

namespace DEP.Service.EncryptionHelpers
{
    public static class UserEncryptionHelper
    {
        public static void Encrypt(User user, IEncryptionService encryptionService)
        {
            if (!string.IsNullOrEmpty(user.UserName))
                user.UserName = encryptionService.Encrypt(user.UserName);

            if (!string.IsNullOrEmpty(user.Name))
                user.Name = encryptionService.Encrypt(user.Name);
        }

        public static void EncryptUpdatableFields(User user, IEncryptionService encryptionService)
        {
            if (!string.IsNullOrEmpty(user.Name))
                user.Name = encryptionService.Encrypt(user.Name);
        }

        public static void Decrypt(User user, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();

            Decrypt(user, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers);
        }

        public static void Decrypt(List<User> users, IEncryptionService encryptionService)
        {
            var decryptedDepartments = new HashSet<Department>();
            var decryptedLocations = new HashSet<Location>();
            var decryptedUsers = new HashSet<User>();

            foreach (var user in users)
            {
                Decrypt(user, encryptionService, decryptedDepartments, decryptedLocations, decryptedUsers);
            }
        }

        public static void DecryptUpdatableFields(User user, IEncryptionService encryptionService)
        {
            if (!string.IsNullOrEmpty(user.Name))
                user.Name = encryptionService.Decrypt(user.Name);
        }

        // Internal shared decrypt method
        private static void Decrypt(
            User user,
            IEncryptionService encryptionService,
            HashSet<Department> decryptedDepartments,
            HashSet<Location> decryptedLocations,
            HashSet<User> decryptedUsers)
        {
            if (user == null) return;

            // Check and decrypt fields only if they haven't been decrypted already
            if (!string.IsNullOrEmpty(user.UserName) && !decryptedUsers.Contains(user))
            {
                user.UserName = encryptionService.Decrypt(user.UserName);
            }

            if (!string.IsNullOrEmpty(user.Name) && !decryptedUsers.Contains(user))
            {
                user.Name = encryptionService.Decrypt(user.Name);
            }

            if (user.Department != null && !decryptedDepartments.Contains(user.Department))
            {
                DepartmentEncryptionHelper.Decrypt(user.Department, encryptionService);
                decryptedDepartments.Add(user.Department);
            }

            if (user.Location != null && !decryptedLocations.Contains(user.Location))
            {
                LocationEncryptionHelper.Decrypt(user.Location, encryptionService);
                decryptedLocations.Add(user.Location);
            }
        }

        public static void Decrypt(UserDashboardViewModel user, IEncryptionService encryptionService)
        {
            if (user == null) return;

            // Decrypt top-level properties
            if (!string.IsNullOrEmpty(user.Name))
                user.Name = encryptionService.Decrypt(user.Name);

            if (!string.IsNullOrEmpty(user.DepartmentName))
                user.DepartmentName = encryptionService.Decrypt(user.DepartmentName);

            if (!string.IsNullOrEmpty(user.LocationName))
                user.LocationName = encryptionService.Decrypt(user.LocationName);

            if (!string.IsNullOrEmpty(user.EducationBossName))
                user.EducationBossName = encryptionService.Decrypt(user.EducationBossName);

            // Decrypt each EducationLeader
            foreach (var leader in user.EducationLeaders ?? Enumerable.Empty<UserViewModel>())
            {
                if (!string.IsNullOrEmpty(leader.Name))
                    leader.Name = encryptionService.Decrypt(leader.Name);

                if (!string.IsNullOrEmpty(leader.DepartmentName))
                    leader.DepartmentName = encryptionService.Decrypt(leader.DepartmentName);

                if (!string.IsNullOrEmpty(leader.LocationName))
                    leader.LocationName = encryptionService.Decrypt(leader.LocationName);
            }

            // Decrypt Persons: EducationalConsultant, EducationLeader, and OperationCoordinator
            DecryptPersons(user.EducationalConsultantPersons, encryptionService);
            DecryptPersons(user.EducationLeaderPersons, encryptionService);
            DecryptPersons(user.OperationCoordinatorPersons, encryptionService);
        }

        private static void DecryptPersons(List<PersonViewModel> persons, IEncryptionService encryptionService)
        {
            if (persons == null) return;

            foreach (var person in persons)
            {
                if (!string.IsNullOrEmpty(person.Name))
                    person.Name = encryptionService.Decrypt(person.Name);

                if (!string.IsNullOrEmpty(person.Initials))
                    person.Initials = encryptionService.Decrypt(person.Initials);

                if (!string.IsNullOrEmpty(person.DepartmentName))
                    person.DepartmentName = encryptionService.Decrypt(person.DepartmentName);

                if (!string.IsNullOrEmpty(person.LocationName))
                    person.LocationName = encryptionService.Decrypt(person.LocationName);
            }
        }
    }

}
