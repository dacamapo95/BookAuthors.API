using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DataTransferObjects
{
    public class NewAuthorDTO
    {
        [Required]
        [FirstUpperCaseAtribbute]
        [StringLength(maximumLength: 120, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Name { get; set; }
    }
}
