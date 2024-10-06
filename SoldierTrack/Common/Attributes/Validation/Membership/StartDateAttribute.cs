namespace SoldierTrack.Web.Common.Attributes.Validation.Membership
{
    using System.ComponentModel.DataAnnotations;

    public class StartDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var startDate = (DateTime?)value;
            var todayLocal = DateTime.Now.Date;

            if (startDate.HasValue && startDate.Value.Date != todayLocal)
            {
                this.ErrorMessage = $"Membership start date should be today!";
                return false;
            }

            return true;
        }
    }
}
