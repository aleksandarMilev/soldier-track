namespace SoldierTrack.Data.Seed
{
    using Models;
    using Models.Enums;

    internal static class ExerciseSeeder
    {
        internal static Exercise[] Seed()
            => new Exercise[]
            {
                new()
                {
                    Id = 1,
                    Name = "Snatch",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://www.muscleandfitness.com/wp-content/uploads/2021/01/the_snatch_2.jpg",
                    Description =
                        "The snatch is one of the two Olympic weightlifting movements. " +
                        "It involves lifting the barbell from the ground to overhead in one fluid motion, " +
                        "demonstrating explosive power, speed, and mobility.",
                    IsForBeginners = false,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 2,
                    Name = "Clean and Jerk",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://barbend.com/wp-content/uploads/2021/02/BarBend-Article-Image-760-x-427-42.jpg",
                    Description =
                        "The clean and jerk is the second Olympic weightlifting movement. " +
                        "This two-part exercise involves cleaning the barbell to the shoulders, " +
                        "then driving it overhead, testing strength, speed, and coordination.",
                    IsForBeginners = false,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 3,
                    Name = "Deadlift",
                    Category = ExerciseCategory.Powerlifting,
                    ImageUrl = "https://i.ytimg.com/vi/lIKyNDZD06g/maxresdefault.jpg",
                    Description =
                        "The deadlift is a powerlifting movement focusing on pulling a loaded barbell from the floor to the hips. " +
                        "It develops overall strength, especially in the posterior chain muscles like glutes, hamstrings, and lower back.",
                    IsForBeginners = true,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 4,
                    Name = "Back Squat",
                    Category = ExerciseCategory.Powerlifting,
                    ImageUrl = "https://squatuniversity.com/wp-content/uploads/2016/02/859835_577024942352334_10881976_o.jpg",
                    Description =
                        "The back squat is a foundational powerlifting exercise where a barbell is positioned on the upper back. " +
                        "It targets the quads, hamstrings, and glutes, building strength and stability in the lower body.",
                    IsForBeginners = true,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 5,
                    Name = "Bench Press",
                    Category = ExerciseCategory.Powerlifting,
                    ImageUrl = "https://completelifter.com/wp-content/uploads/2022/11/Untitled-design-4-optimized.png",
                    Description =
                        "The bench press is one of the main powerlifting lifts, performed by pressing a barbell off the chest while lying down. " +
                        "It targets the chest, triceps, and shoulders, developing upper body pushing strength.",
                    IsForBeginners = true,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 6,
                    Name = "Front Squat",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTBl-92jPe3iigzY0eJHx8P8mnNVrBO4Gq90w&s",
                    Description =
                        "The front squat is a squat variation where the barbell is held across the front of the shoulders. " +
                        "It emphasizes the quads and core, requiring more upper body stability than the back squat.",
                    IsForBeginners = true,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 7,
                    Name = "Overhead Press",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://image.boxrox.com/2021/04/sam-kwant-thruster-overhead-barbell-1024x576.jpg",
                    Description =
                        "The overhead press is a strength exercise performed by pressing a barbell or dumbbells overhead. " +
                        "It primarily targets the shoulders, triceps, and upper chest, developing upper body strength.",
                    IsForBeginners = true,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 8,
                    Name = "Power Clean",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://wodprep.com/wp-content/uploads/2022/05/jerk-dip.jpg",
                    Description =
                        "The power clean is a variation of the clean movement, performed without a full squat. " +
                        "It develops explosive power, speed, and strength, targeting multiple muscle groups.",
                    IsForBeginners = false,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new()
                {
                    Id = 9,
                    Name = "Romanian Deadlift",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://www.catalystathletics.com/articles/images/rdl.jpg",
                    Description =
                        "The Romanian deadlift focuses on the hamstrings and glutes, with less emphasis on the lower back. " +
                        "It’s performed with a slight knee bend, making it a great accessory movement for the deadlift.",
                    IsForBeginners = true,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                 new()
                {
                    Id = 10,
                    Name = "Power Snatch",
                    Category = ExerciseCategory.Weightlifting,
                    ImageUrl = "https://hortonbarbell.com/wp-content/uploads/2022/03/Hang-Power-Snatch-How-To-and-Why.png",
                    Description =
                        "The power snatch is a variation of the snatch movement, performed without a full squat. " +
                        "It develops explosive power, speed, and strength, targeting multiple muscle groups.",
                    IsForBeginners = false,
                    AthleteId = null,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
            };
    }
}
