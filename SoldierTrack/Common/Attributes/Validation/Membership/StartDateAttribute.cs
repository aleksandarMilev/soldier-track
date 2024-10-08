namespace SoldierTrack.Web.Common.Attributes.Validation.Membership
{
    using System.ComponentModel.DataAnnotations;

    public class StartDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var startDate = (DateTime?)value;

            if (startDate.HasValue && startDate.Value.Date.ToUniversalTime() != DateTime.UtcNow.Date)
            {
                this.ErrorMessage = $"Membership start date should be today!";
                return false;
            }

            return true;
        }
    }
}
