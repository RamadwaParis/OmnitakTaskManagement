using System.ComponentModel.DataAnnotations;

namespace OmintakProduction.Validations
{
    /// <summary>
    /// Validation attribute to ensure a DateOnly is not in the past
    /// </summary>
    public class NotPastDateOnlyAttribute : ValidationAttribute
    {
        public bool AllowToday { get; set; } = true;

        public NotPastDateOnlyAttribute()
        {
            ErrorMessage = "Due date cannot be in the past.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Let other validators handle null checks

            if (value is not DateOnly dateOnly)
                return false; // Invalid type

            var today = DateOnly.FromDateTime(DateTime.Today);
            
            if (AllowToday)
            {
                return dateOnly >= today;
            }
            else
            {
                return dateOnly > today;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            if (AllowToday)
            {
                return $"The {name} field cannot be in the past. Please select today's date or a future date.";
            }
            else
            {
                return $"The {name} field must be a future date.";
            }
        }
    }
}
