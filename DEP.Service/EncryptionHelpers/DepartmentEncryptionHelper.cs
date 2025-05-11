using DEP.Repository.Models;
using DEP.Service.Interfaces;

namespace DEP.Service.EncryptionHelpers
{
    public static class DepartmentEncryptionHelper
    {
        public static void Encrypt(Department department, IEncryptionService enc)
        {
            department.Name = enc.Encrypt(department.Name);
        }

        public static void Decrypt(Department department, IEncryptionService enc)
        {
            department.Name = enc.Decrypt(department.Name);
        }
    }
}
