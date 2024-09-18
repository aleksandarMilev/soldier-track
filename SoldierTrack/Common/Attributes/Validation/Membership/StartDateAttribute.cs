namespace SoldierTrack.Web.Common.Attributes.Validation.Membership
{
    using System.ComponentModel.DataAnnotations;

    public class StartDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var startDate = (DateTime?)value;

            if (startDate.HasValue && startDate.Value < DateTime.Today.Date)
            {
                this.ErrorMessage = $"Membership can not start before {DateTime.Today.Date:dd/MM/yyyy}!";
                return false;
            }

            return true;
        }
    }
}
