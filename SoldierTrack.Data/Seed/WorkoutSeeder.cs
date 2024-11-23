namespace SoldierTrack.Data.Seed
{
    using Models;
    using Models.Enums;

    internal static class WorkoutSeeder
    {
        internal static Workout[] Seed()
            => new Workout[]
                {
                    new()
                    {
                        Id = 1,
                        Title = "Murph",
                        DateTime = RoundUpTime(DateTime.UtcNow.AddDays(1)),
                        BriefDescription = 
                            "Murph is a grueling Hero WOD that challenges strength, endurance, and mental toughness, " +
                            "honoring Navy SEAL Lt. Michael P. Murphy.",
                        FullDescription = 
                            "Murph is one of the most iconic Hero WODs in CrossFit, " +
                            "dedicated to Navy SEAL Lt. Michael P. Murphy, who sacrificed his life in combat. " +
                            "This intense workout consists of a 1-mile run, 100 pull-ups, 200 push-ups, 300 air squats, and another 1-mile run, " +
                            "traditionally performed while wearing a 20 lb weight vest. " +
                            "It’s a test of physical endurance and mental resilience, symbolizing the ultimate sacrifice made by Lt. " +
                            "Murphy and many others who serve. Athletes can scale or partition the workout as needed, " +
                            "but the true spirit of Murph is pushing your limits and honoring a hero.",
                        ImageUrl = "https://i0.wp.com/btwb.blog/wp-content/uploads/2018/05/murph_final.jpg?fit=1000%2C715&ssl=1",
                        Category = WorkoutCategory.CrossFit,
                        IsForBeginners = false,
                        MaxParticipants = 15,
                        CurrentParticipants = 0,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new()
                    {
                        Id = 2,
                        Title = "Fran",
                        DateTime = RoundUpTime(DateTime.UtcNow.AddDays(2)),
                        BriefDescription = "Fran is a fast and intense CrossFit workout that tests your power, speed, and conditioning.",
                        FullDescription = 
                            "Fran is a classic CrossFit benchmark workout that combines two movements—thrusters and pull-ups—for time. " +
                            "The rep scheme is 21-15-9, meaning you perform 21 thrusters, 21 pull-ups, then 15 of each, and finally 9 of each. " +
                            "This workout pushes athletes to their limits as they strive to complete it as quickly as possible. " +
                            "Fran is known for its simplicity yet devastating intensity, making it a true test of athletic capacity and mental fortitude.",
                        ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSU2qyjxN9nLMueVzxB79jBW3AUwKmUWQFzDQ&s",
                        Category = WorkoutCategory.CrossFit,
                        IsForBeginners = true,
                        MaxParticipants = 12,
                        CurrentParticipants = 0,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new()
                    {
                        Id = 3,
                        Title = "Cindy",
                        DateTime = RoundUpTime(DateTime.UtcNow.AddDays(3)),
                        BriefDescription = "Cindy is a CrossFit WOD that tests your endurance and bodyweight strength with a 20-minute AMRAP.",
                        FullDescription = 
                            "Cindy is a simple yet challenging CrossFit workout that involves a 20-minute AMRAP (As Many Rounds As Possible) " +
                            "of 5 pull-ups, 10 push-ups, and 15 air squats. It is a great test of endurance, stamina, and bodyweight strength. " +
                            "Athletes aim to complete as many rounds as they can within the 20-minute time frame, focusing on maintaining consistent movement and pacing. " +
                            "Cindy is a versatile workout suitable for athletes of all skill levels, as it can be scaled to match individual fitness abilities.",
                        ImageUrl = "https://crossfitplzen.cz/wp-content/uploads/2020/03/Cindy-WOD-1024x694.jpg",
                        Category = WorkoutCategory.CrossFit,
                        IsForBeginners = true,
                        MaxParticipants = 15,
                        CurrentParticipants = 0,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    }
                };

        private static DateTime RoundUpTime(DateTime dateTime)
        {
            if (dateTime.Minute == 0 && dateTime.Second == 0)
            {
                return dateTime;
            }

            return dateTime.AddHours(1).AddMinutes(-dateTime.Minute).AddSeconds(-dateTime.Second);
        }
    }
}
