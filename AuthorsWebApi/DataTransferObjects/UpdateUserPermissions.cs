using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DataTransferObjects
{
    public class UpdateUserPermissionsDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
