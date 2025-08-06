using OmintakProduction.Models;

namespace OmintakProduction.Services
{
    public class DateValidationService
    {
        /// <summary>
        /// Validates if a DateTime is not in the past
        /// </summary>
        public static bool IsValidFutureDate(DateTime? date, bool allowToday = true)
        {
            if (!date.HasValue) return true; // Null dates are handled by other validators
            
            var today = DateTime.Today;
            return allowToday ? date.Value.Date >= today : date.Value.Date > today;
        }

        /// <summary>
        /// Validates if a DateOnly is not in the past
        /// </summary>
        public static bool IsValidFutureDateOnly(DateOnly date, bool allowToday = true)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return allowToday ? date >= today : date > today;
        }

        /// <summary>
        /// Validates Task due date
        /// </summary>
        public static (bool isValid, string errorMessage) ValidateTaskDueDate(DateTime? dueDate)
        {
            if (!dueDate.HasValue) return (true, string.Empty);
            
            if (!IsValidFutureDate(dueDate, allowToday: true))
            {
                return (false, "Task due date cannot be in the past. Please select today's date or a future date.");
            }
            
            return (true, string.Empty);
        }

        /// <summary>
        /// Validates Project due date
        /// </summary>
        public static (bool isValid, string errorMessage) ValidateProjectDueDate(DateOnly dueDate)
        {
            if (!IsValidFutureDateOnly(dueDate, allowToday: true))
            {
                return (false, "Project due date cannot be in the past. Please select today's date or a future date.");
            }
            
            return (true, string.Empty);
        }

        /// <summary>
        /// Validates Ticket due date
        /// </summary>
        public static (bool isValid, string errorMessage) ValidateTicketDueDate(DateOnly dueDate)
        {
            if (!IsValidFutureDateOnly(dueDate, allowToday: true))
            {
                return (false, "Ticket due date cannot be in the past. Please select today's date or a future date.");
            }
            
            return (true, string.Empty);
        }

        /// <summary>
        /// Validates that start date is before end date
        /// </summary>
        public static (bool isValid, string errorMessage) ValidateDateRange(DateOnly? startDate, DateOnly endDate)
        {
            if (startDate.HasValue && startDate.Value > endDate)
            {
                return (false, "Start date cannot be after the due date.");
            }
            
            return (true, string.Empty);
        }

        /// <summary>
        /// Gets user-friendly error message for past date validation
        /// </summary>
        public static string GetPastDateErrorMessage(string fieldName, bool allowToday = true)
        {
            if (allowToday)
            {
                return $"{fieldName} cannot be in the past. Please select today's date or a future date.";
            }
            else
            {
                return $"{fieldName} must be a future date.";
            }
        }
    }
}
