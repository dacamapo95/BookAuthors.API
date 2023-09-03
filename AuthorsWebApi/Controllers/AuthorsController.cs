using AuthorsWebApi.Database;
using AuthorsWebApi.DataTransferObjects;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/authors")] //api/authors
    //[Route("api/[controller]")] //api/Authors según nombre de controller
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext applicationDbContext,
                                 IMapper mapper)
        {
            dbContext = applicationDbContext;
            this.mapper = mapper;
        }

        [HttpGet(Name = "GetAuthors")] //api/Authors
        //[HttpGet("AuthorsList")] //api/Authors/list
        //[HttpGet("/AuthorsList")] // /AuthorsList
        //[ResponseCache(Duration = 10)] //Proximas peticiones después de 10s se sirven del cache.
        [AllowAnonymous] //Usuarios no autenticados podrán consumir
        public async Task<ActionResult<Author[]>> Get()
            => await dbContext.Authors//.Include(author => author.Books)
            .ToArrayAsync();
        
        //[HttpGet("first")] //api/authors/first
        //public async Task<ActionResult<Author>> GetFirstAuthor() 
        //    => await dbContext.Authors.FirstOrDefaultAsync();

        [HttpGet("{id:int}", Name = "GetAuthorById")]
        public async Task<ActionResult<AuthorDTO>> Get(int id)
        {
            Author author = await dbContext.Authors.Include(author => author.Books)
                                                   .ThenInclude(association => association.Book)
                                                   .FirstOrDefaultAsync(author => author.AuthorId == id);

            if (author is null) return NotFound();
            return mapper.Map<AuthorDTO>(author);
        }

        [HttpGet("{name}", Name = "GetAuthorsByName")]
        public async Task<ActionResult<AuthorDTO[]>> Get(string name) =>
            mapper.Map<AuthorDTO[]>(await dbContext.Authors.Where(author => author.Name.Contains(name)).ToArrayAsync());

        [HttpPost(Name = "CreatetAuthor")]
        public async Task<ActionResult> Post([FromBody] NewAuthorDTO author)
        {
            if (await dbContext.Authors.AnyAsync(dbAuthor => dbAuthor.Name == author.Name))
                return BadRequest($"There's an Author saved with Name: {author.Name}");

            Author Author = mapper.Map<Author>(author);  
            dbContext.Add(Author);
            await dbContext.SaveChangesAsync();

            AuthorDTO AuthorDto = mapper.Map<AuthorDTO>(Author);  

            return CreatedAtRoute("GetAuthorById", new { id = Author.AuthorId} ,AuthorDto);
        }

        [HttpPut("{id:int}", Name = "UpdateAuthor")] //api/autores/id
        public async Task<ActionResult> Put(NewAuthorDTO authorDto, int id)
        {
            if (! await dbContext.Authors.AnyAsync(author => author.AuthorId == id)) return NotFound();

            Author Author = mapper.Map<Author>(authorDto);
            Author.AuthorId = id;

            dbContext.Update(Author);
            await dbContext.SaveChangesAsync();
            return NoContent();    
        }

        [HttpDelete("{id:int}", Name = "DeleteAuthor")] //api/authors/id   
        public async Task<ActionResult> Delete(int id)
        {
            Author author = await dbContext.Authors.FirstOrDefaultAsync(author => author.AuthorId == id);

            if (author is not null)
            {
                dbContext.Remove(author);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
           
            return NotFound($"No se encontró autor con id: {id}.");
        }
    }
}