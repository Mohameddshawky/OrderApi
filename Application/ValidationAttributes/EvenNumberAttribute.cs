using System.ComponentModel.DataAnnotations;

namespace Application.ValidationAttributes;

public class EvenNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int intValue)
        {
            if (intValue % 2 != 0)
            {
                return new ValidationResult(ErrorMessage ?? "The field must be an even number.");
            }
        }
        else if (value is long longValue)
        {
            if (longValue % 2 != 0)
            {
                return new ValidationResult(ErrorMessage ?? "The field must be an even number.");
            }
        }
        
        return ValidationResult.Success;
    }
}
