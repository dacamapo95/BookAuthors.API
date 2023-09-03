using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorsWebApi.Entities
{
    public class Author 
    {
        public int AuthorId { get; set; }

        [Required]
        [FirstUpperCaseAtribbute]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Name { get; set; }

        public List<Author_Book> Books { get; set; }
    }

}
