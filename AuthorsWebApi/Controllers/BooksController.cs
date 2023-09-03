using AuthorsWebApi.Database;
using AuthorsWebApi.DataTransferObjects;
using AuthorsWebApi.Entities;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/Books")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            context = applicationDbContext;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "GetBookById")]
        public async Task<ActionResult<BookDTO>> Get(int id) =>
             mapper.Map<BookDTO>(await context.Books
                                              .Include(book => book.Authors)
                                              .ThenInclude(association => association.Author)
                                              .Include(book => book.Comments)
                                              .FirstOrDefaultAsync(book => book.BookId == id));
        [HttpPost(Name = "CrateBook")]
        public async Task<ActionResult> Post(NewBookDTO newBookDto)
        {
            if (newBookDto.AuthorIds is null) return BadRequest("Creating a book without authors is not possible.");
            var authorsIds = await context.Authors
                                          .Where(author => newBookDto.AuthorIds.Contains(author.AuthorId))
                                          .Select(author => author.AuthorId)
                                          .ToListAsync();

            if (authorsIds.Count != newBookDto.AuthorIds.Count) return BadRequest("There is an author who doesn't exist.");

            var book = mapper.Map<Book>(newBookDto);
            context.Add(book);
            await context.SaveChangesAsync();

            book.Authors.ForEach(association => context.Entry(association).Reference(ab => ab.Author).Load());

            var bookDto = mapper.Map<BookDTO>(book);

            return CreatedAtRoute("GetBookById", new { id = book.BookId }, bookDto);
        }

        [HttpPut("{id:int}", Name = "UpdateBook")]
        public async Task<ActionResult> Put(int id, NewBookDTO bookDTO)
        {
            var Book = await context.Books
                                    .Include(book => book.Authors)
                                    .FirstOrDefaultAsync(book => book.BookId == id);

            if (Book == null) return NotFound();

            Book = mapper.Map(bookDTO, Book);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "PatchBook")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> jsonPatchDocument)
        {
            if (jsonPatchDocument == null) return BadRequest();

            var Book = await context.Books.FirstOrDefaultAsync(book => book.BookId == id);

            if (Book == null) return NotFound();

            var BookDto = mapper.Map<BookPatchDTO>(Book);
            jsonPatchDocument.ApplyTo(BookDto, ModelState);

            if (!TryValidateModel(BookDto)) return BadRequest(ModelState);

            mapper.Map(BookDto, Book);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteBook")] //api/authors/id   
        public async Task<ActionResult> Delete(int id)
        {
            Book book = await context.Books.FirstOrDefaultAsync(book => book.BookId == id);

            if (book is not null)
            {
                context.Remove(book);
                await context.SaveChangesAsync();
                return Ok();
            }

            return NotFound($"No se encontró autor con id: {id}.");
        }
    }
}