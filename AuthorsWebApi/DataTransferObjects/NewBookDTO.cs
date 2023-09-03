using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DataTransferObjects
{
    public class NewBookDTO
    {
        [FirstUpperCaseAtribbute]
        [StringLength(maximumLength: 250, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        public List<int> AuthorIds { get; set; }
    }
}
