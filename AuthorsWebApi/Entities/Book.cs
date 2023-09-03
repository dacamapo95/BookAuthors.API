using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Entities
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [FirstUpperCaseAtribbute]
        [StringLength(maximumLength: 250, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Title { get; set; }

        public DateTime? PublicationDate { get; set; }

        public List<BookComment> Comments { get; set; }

        public List<Author_Book> Authors { get; set; }

        public int Order { get; set; }
    }
}
