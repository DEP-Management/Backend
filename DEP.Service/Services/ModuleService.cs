using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using DEP.Service.EncryptionHelpers;
using DEP.Service.Interfaces;
using DEP.Service.ViewModels;

namespace DEP.Service.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository repo;
        private readonly ICourseRepository courseRepo;
        private readonly IEncryptionService encryptionService;
        public ModuleService(IModuleRepository repo, ICourseRepository courseRepo, IEncryptionService encryptionService)
        {
            this.repo = repo; this.courseRepo = courseRepo;
            this.encryptionService = encryptionService;
        }

        public async Task<bool> AddModule(Module module)
        {
            return await repo.AddModule(module);
        }

        public async Task<bool> DeleteModule(int id)
        {
            return await repo.DeleteModule(id);
        }

        public async Task<List<Module>> GetModules()
        {
            return await repo.GetModules();
        }

        public async Task<List<ModuleWithCourseViewModel>> GetModulesWithCourse()
        {
            var re = await repo.GetModules();
            var modules = new List<ModuleWithCourseViewModel>();

            foreach (var module in re)
            {
                var courses = await courseRepo.GetCourseWithPerson(module.ModuleId);

                var excelModules = new ModuleWithCourseViewModel
                {
                    ModuleId = module.ModuleId,
                    Name = module.Name,
                    Description = module.Description,
                    Courses = courses,
                };
                modules.Add(excelModules);
            }

            PersonEncryptionHelper.DecryptCoursesWithPersons(modules, encryptionService);

            return modules;
        }

        public async Task<List<ModuleWithCourseViewModel>> GetModuleWithCourseInList(int id)
        {
            var re = await repo.GetModuleById(id);
            var modules = new List<ModuleWithCourseViewModel>();

            var courses = await courseRepo.GetCourseWithPerson(re.ModuleId);

            var excelModules = new ModuleWithCourseViewModel
            {
                ModuleId = re.ModuleId,
                Name = re.Name,
                Description = re.Description,
                Courses = courses,
            };
            modules.Add(excelModules);

            PersonEncryptionHelper.DecryptCoursesWithPersons(modules, encryptionService);

            return modules;
        }

        public async Task<bool> UpdateModule(Module module)
        {
            return await repo.UpdateModule(module);
        }
    }
}
