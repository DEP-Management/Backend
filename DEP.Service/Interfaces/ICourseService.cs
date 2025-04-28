using DEP.Repository.Models;

namespace DEP.Service.Interfaces
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCourses();
        Task<List<Course>> GetCoursesByModuleId(int moduleId);
        Task<List<Course>> GetCoursesByModuleIdAndUserId(int moduleId, int userId);
        Task<List<Course>> GetSelectedCourseById(int id);
        Task<Course> GetCourseById(int id);
        Task<bool> AddCourse(Course personModule);
        Task<bool> UpdateCourse(Course personModule);
        Task<bool> DeleteCourse(int id);
    }
}
