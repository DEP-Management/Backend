using DEP.Repository.Context;
using DEP.Repository.Interfaces;
using DEP.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace DEP.Repository.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DatabaseContext context;
        public BookRepository(DatabaseContext context) { this.context = context; }

        public async Task<bool> AddBook(Book book)
        {
            // Check if the Module object is currently being tracked in 'Local', This doesn't query the DB.
            var existingModule = context.Modules.Local.FirstOrDefault(x => x.ModuleId == book.ModuleId);

            if (existingModule == null)
            {
                // Attach to Book using ModuleId so the book uses an existing Module. Preventing EntityFramework from inserting an entirely new module into DB.
                existingModule = new Module { ModuleId = book.ModuleId };
                context.Modules.Attach(existingModule);
            }

            book.Module = existingModule;
            context.Books.Add(book);
            var result = await context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteBook(int id)
        {
            var book = await context.Books.FindAsync(id);

            if (book is null)
            {
                return false;
            }

            context.Books.Remove(book);
            var result = await context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Book> GetBook(int id)
        {
            var book = await context.Books
                .Where(b => b.BookId == id)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Name = b.Name,
                    Amount = b.Amount,
                    ModuleId = b.ModuleId,
                    Module = b.Module == null ? null : new Module
                    {
                        ModuleId = b.Module.ModuleId,
                        Name = b.Module.Name
                    }
                })
                .FirstOrDefaultAsync();

            return book;
        }

        public async Task<List<Book>> GetBooks()
        {
            var books = await context.Books
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Name = b.Name,
                    Amount = b.Amount,
                    ModuleId = b.ModuleId,
                    Module = b.Module == null ? null : new Module
                    {
                        ModuleId = b.Module.ModuleId,
                        Name = b.Module.Name
                    }
                })
                .ToListAsync();

            return books;
        }


        public async Task<bool> UpdateBook(Book book)
        {
            context.Entry(book).State = EntityState.Modified;
            var result = await context.SaveChangesAsync();

            return result > 0;
        }
    }
}
