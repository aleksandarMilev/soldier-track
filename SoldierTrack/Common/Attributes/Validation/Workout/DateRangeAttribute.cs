namespace SoldierTrack.Web.Common.Attributes.Validation.Workout
{
    using System.ComponentModel.DataAnnotations;

    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var dateProperty = validationContext.ObjectType.GetProperty("Date");
            if (dateProperty == null)
            {
                return new ValidationResult("Date property not found.");
            }

            var timeProperty = validationContext.ObjectType.GetProperty("Time");
            if (timeProperty == null)
            {
                return new ValidationResult("Time property not found.");
            }

            var dateValue = dateProperty.GetValue(validationContext.ObjectInstance, null);
            var timeValue = timeProperty.GetValue(validationContext.ObjectInstance, null);

            if (dateValue == null || dateValue is not DateTime date)
            {
                return new ValidationResult("Invalid date.");
            }

            if (timeValue == null || timeValue is not TimeSpan time)
            {
                return new ValidationResult("Invalid time.");
            }

            var maxDate = DateTime.Now.Date.AddMonths(1);
            if (date < DateTime.Now.Date || date > maxDate)
            {
                return new ValidationResult($"Date must be between today and {maxDate.ToShortDateString()}.");
            }

            if (date.Date == DateTime.Now.Date)
            {
                var currentTime = DateTime.Now.TimeOfDay;
                if (time < currentTime)
                {
                    return new ValidationResult("Workout time cannot be in the past.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
