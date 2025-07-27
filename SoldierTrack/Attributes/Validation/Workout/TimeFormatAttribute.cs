namespace SoldierTrack.Web.Attributes.Validation.Workout
{
    using System.ComponentModel.DataAnnotations;

    public class TimeFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var timeProp = validationContext.ObjectType.GetProperty("Time");
            if (timeProp == null)
            {
                return new ValidationResult("Time property not found.");
            }

            var timeValue = timeProp.GetValue(validationContext.ObjectInstance, null);
            if (timeValue == null || timeValue is not TimeSpan time)
            {
                return new ValidationResult("Invalid date.");
            }

            if (time.Minutes != 0 || time.Seconds != 0)
            {
                return new ValidationResult($"The time must be a whole hour (e.g., 09:00, 10:00).");
            }

            return ValidationResult.Success!;
        }
    }
}
