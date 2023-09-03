using AuthorsWebApi.Database;
using AuthorsWebApi.DataTransferObjects;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/Books/{bookId:int}/comments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> identityManager;

        public CommentsController(
            ApplicationDbContext applicationDbContext, 
            IMapper mapper,
            UserManager<IdentityUser> identityManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.mapper = mapper;
            this.identityManager = identityManager;
        }

        [HttpGet(Name = "GetBookComments")]
        public async Task<ActionResult<BookCommentDTO[]>> Get(int bookId)
        {
            if (! await applicationDbContext.Books.AnyAsync(book => book.BookId == bookId)) return NotFound();

            return mapper.Map<BookCommentDTO[]>(await applicationDbContext.BookComments.Where(comment => comment.BookId == bookId).ToArrayAsync());
        }

        [HttpGet("{id:int}", Name = "GetCommmentById")]
        public async Task<ActionResult<BookCommentDTO>> GetCommmentById(int id)
        {
            var Comment = await applicationDbContext.BookComments.FirstOrDefaultAsync(comment => comment.BookCommentId == id);

            if (Comment == null) return NotFound();

            return mapper.Map<BookCommentDTO>(Comment);
        }

        [HttpPost(Name = "CreateComment")]
        public async Task<ActionResult> Post(int bookId, NewBookCommentDTO commentDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var user = await identityManager.FindByEmailAsync(emailClaim.Value);

            Book book = await applicationDbContext.Books.FirstOrDefaultAsync(book => book.BookId == bookId);

            if (book == null) return NotFound();

            BookComment bookComment = mapper.Map<BookComment>(commentDTO);
            bookComment.BookId = bookId;
            bookComment.UserId = user.Id;
            applicationDbContext.BookComments.Add(bookComment);
            await applicationDbContext.SaveChangesAsync();
            BookCommentDTO bookCommentDto = mapper.Map<BookCommentDTO>(bookComment);

            return CreatedAtRoute("GetCommmentById", new { id = bookCommentDto.BookCommentId, bookId = bookId}, bookCommentDto);
        }

        [HttpPut("{id:int}", Name = "UpdateComment")]
        public async Task<ActionResult> Put(int id, int bookId, BookCommentDTO bookCommentDTO)

        { 
            if (!(await applicationDbContext.BookComments.AnyAsync(comment => comment.BookCommentId == id) &&
                  await applicationDbContext.Books.AnyAsync(book => book.BookId == bookId))) return NotFound();

            var Comment = mapper.Map<BookComment>(bookCommentDTO);
            Comment.BookCommentId = id;
            Comment.BookId = bookId;
            applicationDbContext.Update(Comment);
            await applicationDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
