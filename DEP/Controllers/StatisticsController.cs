using DEP.Repository.Models;
using DEP.Repository.ViewModels;
using DEP.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService service;

        public StatisticsController(IStatisticsService service)
        {
            this.service = service;
        }

        [HttpGet("personsperdepartment/module/{moduleId:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetPersonPerDepartmentByModule(int moduleId)
        {
            return Ok(await service.GetPersonsPerDepartmentByModule(moduleId));
        }

        [HttpGet("personsperdepartment"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetPersonsPerDepartment()
        {
            return Ok(await service.GetPersonsPerDepartment());
        }

        [HttpGet("personsperlocation"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetPersonsPerLocation()
        {
            return Ok(await service.GetPersonsPerLocation());
        }

        [HttpGet("coursestatuscount/module/{moduleId:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetCourseStatusCountByModule(int moduleId)
        {
            return Ok(await service.GetCourseStatusCountByModule(moduleId));
        }

        [HttpPost("coursestatuscount/filter"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetCourseStatusCountFiltered([FromBody] CourseStatusFilterViewModel filter)
        {
            var result = await service.GetCourseStatusCountFiltered(filter);
            return Ok(result);
        }

        [HttpGet("personsperdepartmentandlocation"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetPersonsPerDepartmentAndLocation()
        {
            return Ok(await service.GetPersonsPerDepartmentAndLocation());
        }

        [HttpGet("personspermodule"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetPersonsPerModule()
        {
            return Ok(await service.GetPersonsPerModuleAsync());
        }

        [HttpGet("personspermoduleincludingempty"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Human_Resources) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetPersonsPerModuleIncludingEmpty()
        {
            return Ok(await service.GetPersonsPerModuleIncludingEmptyModulesAsync());
        }
    }
}
