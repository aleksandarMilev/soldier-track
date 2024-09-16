
namespace SoldierTrack.Data.Constants
{
    public static class ModelsConstraints
    {
        public static class AthleteConstraints
        {
            public const int NamesMinLength = 2;
            public const int NamesMaxLength = 50;

            public const int PhoneLength = 10;

            public const int EmailMinLength = 5;
            public const int EmailMaxLength = 100;


            public const int UserIdLength = 36;
        }

        public static class WorkoutConstraints
        {
            public const int TitleMinLength = 2;
            public const int TitleMaxLength = 50;

            public const int BriefDescriptionMinLength = 10;
            public const int BriefDescriptionMaxLength = 100;

            public const int FullDescriptionMinLength = 50;
            public const int FullDescriptionMaxLength = 2_000;

            public const int ImageUrlMinLength = 10;
            public const int ImageUrlMaxLength = 255;

            public const int ParticipantsMinValue = 1;
            public const int ParticipantsMaxValue = 15;

            public const int CurrentParticipantsMinValue = 0;
        }

        public static class MembershipConstraints
        {
            public const int TotalCountMinValue = 0;
            public const int TotalCountMaxValue = 31;

            public const int PriceMinValue = 0;
            public const int PriceMaxValue = 300;
        }

        public static class CategoryConstraints
        {
            public const int NameMaxLength = 50;
        }

        public static class ExerciseConstraints
        {
            public const int NameMaxLength = 50;
        }
    }
}
