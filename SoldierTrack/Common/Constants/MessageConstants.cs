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

        public const string JoinSuccess = "You have successfully joined in the workout!";

        public const string AthleteLeaveSuccess = "You have successfully left the workout!";

        public const string WorkoutNotFound = "Workout is not found!";

        public const string WorkoutEdited = "Workout is now edited!";

        //athlete
        public const string AthleteWithSameNumberExists = "An athlete with number {0} is already registered!";

        public const string AthleteSuccessRegister = "You have successfully registered as athlete!";

        public const string AthleteEditHimself = "You have edited your profile successfully!";

        public const string AthleteDeleteHimself = "Your profile was deleted successfully!";

        public const string AthleteAlreadyJoined = "Athlete has already joined in {0}!";

        //membership
        public const string MembershipRequested = "You have successfully requested yor membership!";

        public const string MembershipApproved = "Membership successfully approved!";

        public const string MembershipRejected = "Membership successfully rejected!";

        public const string MembershipEdited = "Membership successfully edited!";

        public const string MembershipDeleted = "Membership successfully deleted!";

        public const string MembershipExpired = "Yor membership have expired! Please, request a new one.";

        public const string JoinUtilFailure = "A problem has occurred while joining. Please, try again. If the problem continues, call us on 0000000000.";

        public const string LeaveUtilFailure = "A problem has occurred, you are not removed from the workout. Please, try again. If the problem continues, call us on 0000000000.";

        //admin
        public const string AdminEditAthlete = "Athlete edited successfully!";

        public const string AdminDeleteAthlete = "Athlete deleted successfully!";

        public const string AdminAddedAthlete = "You have successfully added the athlete in {0}!";

        public const string AdminLeaveSuccess = "You have successfully removed the athlete!";

        public const string AdminRequestMembership = "Admins can not request memberships";

        public const string AdminGetAchievement = "Admins have not achievements";

        //achievement
        public const string InvalidWeightLiftedValue = "Weight lifted should be between {1} and {2} kilos!";

        public const string InvalidRepetitionsValue = "Repetitions should be between {1} and {2}!";

        public const string PRSuccessfullyAdded = "PR successfully added!";

        public const string AchievementAlreadyAdded = "You have already added your PR for this movement. If you want to improve it, please, go back on 'My Achievements' and click on the 'Update' button.";

        public const string AchievementEdited = "You have successfully edited your achievement!";

        public const string AchievementDeleted = "You have successfully reset your achievement!";

        //exercise
        public const string ExerciseCreated = "Exercise is successfully created!";

        public const string ExerciseEdited = "Exercise is successfully edited!";

        public const string ExerciseDeleted = "Exercise is successfully deleted!";
    }
}
