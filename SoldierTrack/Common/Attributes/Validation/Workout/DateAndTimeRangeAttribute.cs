namespace SoldierTrack.Web.Common.Attributes.Validation.Workout
{
    using System.ComponentModel.DataAnnotations;
    using SoldierTrack.Web.Common.Constants.WorkoutTimes;


    public class DateAndTimeRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var dateProperty = validationContext.ObjectType.GetProperty("Date");

            if (dateProperty == null)
            {
                return new ValidationResult("Date property not found.");
            }

            var dateValue = dateProperty.GetValue(validationContext.ObjectInstance, null);

            if (dateValue == null || dateValue is not DateTime date)
            {
                return new ValidationResult("Invalid date.");
            }

            var maxDate = DateTime.Now.Date.AddMonths(1);

            if (date < DateTime.Now.Date || date > maxDate)
            {
                return new ValidationResult($"Date must be between today and {maxDate.ToShortDateString()}.");
            }

            if (value is string timeString)
            {
                if (!WorkoutTimes.AllowedTimes.Contains(timeString))
                {
                    return new ValidationResult("The workout start time is not valid.");
                }

                if (TimeSpan.TryParse(timeString, out TimeSpan time))
                {
                    var combinedDateTime = date.Add(time);

                    if (combinedDateTime < DateTime.Now)
                    {
                        return new ValidationResult("The combination of date and time cannot be in the past.");
                    }

                    return ValidationResult.Success!;
                }

                return new ValidationResult("Invalid time format.");
            }

            return new ValidationResult("Invalid time.");
        }
    }
}
