using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Entities
{
    public class Author_Book
    {
        public int BookId { get; set; }

        public int AuthorId { get; set; }

        public int Order { get; set; }

        public Book Book { get; set; }

        public Author Author { get; set; }
    }
}
