namespace SoldierTrack.Web.Common.Constants
{
    public static class MessageConstants
    {
        public const string LengthError = "The field length should be between {2} and {1} characters long!";

        public const string RequiredError = "The {0} field is required!";

        public const string InvalidCategoryName = "Invalid category name!";

        public const string InvalidPhoneFormat = "Invalid phone number format!";

        //workout
        public const string WorkoutAlreadyListed = "Workout is already listed at {0}!";

        public const string WorkoutDeletedSuccessfully = "You have successfully deleted the workout!";

        public const string WorkoutCreated = "You have successfully added the workout!";

        //athlete
        public const string AthleteWithSameNumberExists = "An athlete with number {0} is already registered!";

        public const string AthleteSuccessRegister = "You have successfully registered as athlete!";

        public const string AthleteEditHimself = "You have edited your profile successfully!";

        public const string AthleteDeleteHimself = "Your profile was deleted successfully!";

        //membership
        public const string MembershipRequested = "You have successfully requested yor membership!";

        public const string MembershipApproved = "Membership successfully approved!";

        public const string MembershipRejected = "Membership successfully rejected!";

        public const string MembershipEdited = "Membership successfully edited!";

        public const string MembershipDeleted = "Membership successfully deleted!";

        //admin
        public const string AdminEditAthlete = "Athlete edited successfully!";

        public const string AdminDeleteAthlete = "Athlete deleted successfully!";
    }
}
