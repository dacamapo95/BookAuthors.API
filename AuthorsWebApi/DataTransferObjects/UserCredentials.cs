using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DataTransferObjects
{
    public class UserCredentials
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
