using DEP.Repository.Models;
using DEP.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace DEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService service;
        public CoursesController(ICourseService service) { this.service = service; }

        [HttpGet, Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetAllCourses()
        {
            return Ok(await service.GetAllCourses());
        }

        [HttpGet("module/{moduleId:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Uddannelseschef) + "," +
    nameof(UserRole.Uddannelsesleder) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetCoursesByModuleId(int moduleId)
        {
            return Ok(await service.GetCoursesByModuleId(moduleId));
        }

        //[HttpGet("module/{moduleId:int}/user/{userId:int}"), Authorize]
        //public async Task<IActionResult> GetCoursesByModuleIdAndUserId(int moduleId, int userId)
        //{
        //    return Ok(await service.GetCoursesByModuleIdAndUserId(moduleId, userId));
        //}

        [HttpGet("{id:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Uddannelseschef) + "," +
    nameof(UserRole.Uddannelsesleder) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetCourseById(int id)
        {
            return Ok(await service.GetCourseById(id));
        }
        
        [HttpGet("selcted/{id:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Uddannelseschef) + "," +
    nameof(UserRole.Uddannelsesleder) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetSelctedCourseById(int id)
        {
            return Ok(await service.GetSelectedCourseById(id));
        }

        [HttpDelete("{id:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            return Ok(await service.DeleteCourse(id));
        }

        [HttpPost, Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> AddCourse(Course course)
        {
            return Ok(await service.AddCourse(course));
        }

        [HttpPut, Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> UpdateCourse(Course course)
        {
            return Ok(await service.UpdateCourse(course));
        }
    }
}
