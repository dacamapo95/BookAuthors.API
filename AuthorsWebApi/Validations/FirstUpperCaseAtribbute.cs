using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Validations
{
    public class FirstUpperCaseAtribbute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value?.ToString())) return ValidationResult.Success;

            if (!char.IsUpper(value.ToString().First())) 
                return new ValidationResult($"The Field {value.ToString()} must start UpperCase.");
            
            return ValidationResult.Success;
        }
    }
}
