using System;
using DEP.Repository.Models;
using DEP.Service.Interfaces;
using File = DEP.Repository.Models.File;

namespace DEP.Service.EncryptionHelpers
{
    public static class FileEncryptionHelper
    {
        public static void Decrypt(List<File> files, IEncryptionService encryptionService)
        {
            var decryptedPersons = new HashSet<Person>();

            foreach (var file in files)
            {
                Decrypt(file, encryptionService, decryptedPersons);
            }
        }

        // Internal shared decrypt method
        private static void Decrypt(
            File file,
            IEncryptionService encryptionService,
            HashSet<Person> decryptedPersons)
        {

            if (file.Person != null && !decryptedPersons.Contains(file.Person))
            {
                if (file.Person.Name != null)
                    file.Person.Name = encryptionService.Decrypt(file.Person.Name);

                if (file.Person.Initials != null)
                    file.Person.Initials = encryptionService.Decrypt(file.Person.Initials);

                decryptedPersons.Add(file.Person);
            }

            //if (user == null) return;

            //// Check and decrypt fields only if they haven't been decrypted already
            //if (!string.IsNullOrEmpty(user.UserName) && !decryptedUsers.Contains(user))
            //{
            //    user.UserName = encryptionService.Decrypt(user.UserName);
            //}

            //if (!string.IsNullOrEmpty(user.Name) && !decryptedUsers.Contains(user))
            //{
            //    user.Name = encryptionService.Decrypt(user.Name);
            //}

            //if (user.Department != null && !decryptedDepartments.Contains(user.Department))
            //{
            //    DepartmentEncryptionHelper.Decrypt(user.Department, encryptionService);
            //    decryptedDepartments.Add(user.Department);
            //}

            //if (user.Location != null && !decryptedLocations.Contains(user.Location))
            //{
            //    LocationEncryptionHelper.Decrypt(user.Location, encryptionService);
            //    decryptedLocations.Add(user.Location);
            //}
        }
    }
}
