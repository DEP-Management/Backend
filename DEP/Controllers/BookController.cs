using DEP.Repository.Models;
using DEP.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService service;
        public BookController(IBookService service) { this.service = service; }

        [HttpGet("{id:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetBook(int id)
        {
            return Ok(await service.GetBook(id));
        }

        [HttpGet, Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> GetBooks()
        {
            return Ok(await service.GetBooks());
        }

        [HttpPost, Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> AddBook(Book book)
        {
            return Ok(await service.AddBook(book));
        }

        [HttpPut, Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            return Ok(await service.UpdateBook(book));
        }

        [HttpDelete("{id:int}"), Authorize(Roles =
    nameof(UserRole.Driftskoordinator) + "," +
    nameof(UserRole.Pædagogisk_konsulent) + "," +
    nameof(UserRole.Controller) + "," +
    nameof(UserRole.Administrator))]
        public async Task<IActionResult> DeleteBook(int id)
        {
            return Ok(await service.DeleteBook(id));
        }
    }
}
