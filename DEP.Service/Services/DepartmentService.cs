using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.EncryptionHelpers;
using DEP.Service.Interfaces;

namespace DEP.Service.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository depRepository;
        private readonly IEncryptionService encryptionService;

        public DepartmentService(IDepartmentRepository depRepository, IEncryptionService encryptionService)
        {
            this.depRepository = depRepository;
            this.encryptionService = encryptionService;
        }

        public async Task<List<Department>> GetDepartments()
        {
            var departments = await depRepository.GetDepartments();
            departments.ForEach(d => DepartmentEncryptionHelper.Decrypt(d, encryptionService));
            return departments;
        }

        public async Task<bool> AddDepartment(Department department)
        {
            DepartmentEncryptionHelper.Encrypt(department, encryptionService);
            return await depRepository.AddDepartment(department);
        }

        public async Task<bool> UpdateDepartment(Department department)
        {
            DepartmentEncryptionHelper.Encrypt(department, encryptionService);
            return await depRepository.UpdateDepartment(department);
        }

        public async Task<bool> DeleteDepartment(int id)
        {
            return await depRepository.DeleteDepartment(id);
        }
    }
}
