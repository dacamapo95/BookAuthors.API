using Microsoft.AspNetCore.Identity;

namespace AuthorsWebApi.Entities
{
    public class BookComment
    {
        public int BookCommentId { get; set; }

        public string Content { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }

        public string UserId { get; set; }

        public IdentityUser User { get; set; }
    }
}
