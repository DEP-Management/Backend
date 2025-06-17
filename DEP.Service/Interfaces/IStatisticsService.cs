using DEP.Repository.ViewModels;
using DEP.Service.ViewModels.Statistic;

namespace DEP.Service.Interfaces
{
    public interface IStatisticsService
    {
        Task<List<PersonPerDepartmentViewModel>> GetPersonsPerDepartmentByModule(int moduleId);
        Task<List<PersonPerDepartmentViewModel>> GetPersonsPerDepartment();
        Task<List<PersonPerLocationViewModel>> GetPersonsPerLocation();
        Task<List<CourseStatusCountViewModel>> GetCourseStatusCountByModule(int moduleId);
        Task<List<CourseStatusCountViewModel>> GetCourseStatusCountFiltered(CourseStatusFilterViewModel filter);
        Task<PersonPerDepartmentAndLocationViewModel> GetPersonsPerDepartmentAndLocation();
        Task<List<PersonPerDepartmentViewModel>> GetPersonsPerModuleAsync();
        Task<List<PersonPerDepartmentViewModel>> GetPersonsPerModuleIncludingEmptyModulesAsync();
    }
}
