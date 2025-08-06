using System.ComponentModel.DataAnnotations;

namespace OmintakProduction.Validations
{
    /// <summary>
    /// Validation attribute to ensure a DateTime is not in the past
    /// </summary>
    public class NotPastDateAttribute : ValidationAttribute
    {
        public bool AllowToday { get; set; } = true;

        public NotPastDateAttribute()
        {
            ErrorMessage = "Due date cannot be in the past.";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true; // Let other validators handle null checks

            DateTime dateToCheck;

            // Handle different date types
            if (value is DateTime dateTime)
            {
                dateToCheck = dateTime.Date;
            }
            else if (value is DateOnly dateOnly)
            {
                dateToCheck = dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                return false; // Invalid type
            }

            var today = DateTime.Today;
            
            if (AllowToday)
            {
                return dateToCheck >= today;
            }
            else
            {
                return dateToCheck > today;
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
