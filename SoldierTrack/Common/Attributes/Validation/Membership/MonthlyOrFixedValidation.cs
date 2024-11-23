namespace SoldierTrack.Web.Common.Attributes.Validation.Membership
{
    using System.ComponentModel.DataAnnotations;

    using Models.Membership;

    public class MonthlyOrFixedValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not MembershipFormModel model)
            {
                return false;
            }

            if (model.IsMonthly &&
                model.TotalWorkoutsCount.HasValue && model.TotalWorkoutsCount.Value != 0)
            {
                this.ErrorMessage = "Membership can not be both indefinite and with fixed workouts count!";
                return false;
            }

            if (!model.IsMonthly &&
                model.TotalWorkoutsCount.HasValue && model.TotalWorkoutsCount.Value <= 0)
            {
                this.ErrorMessage = "Membership can not be both indefinite and with fixed workouts count!";
                return false;
            }

            if (!model.IsMonthly && model.TotalWorkoutsCount == null)
            {
                this.ErrorMessage = "Membership should be either with fixed workouts count or indefinite!";
                return false;
            }

            return true;
        }
    }
}
