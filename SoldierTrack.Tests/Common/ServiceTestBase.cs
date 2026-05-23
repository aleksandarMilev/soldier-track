namespace SoldierTrack.Tests.Common
{
    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;

    public abstract class ServiceTestBase : IDisposable
    {
        protected readonly ApplicationDbContext data;

        protected ServiceTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            this.data = new ApplicationDbContext(options);
        }

        public void Dispose()
            => GC.SuppressFinalize(this);

        protected Athlete SeedAthlete(
            string id = "athlete-1",
            string firstName = "John",
            string lastName = "Doe",
            string email = "john@example.com")
        {
            var athlete = new Athlete
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                UserName = email,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper()
            };

            this.data.Add(athlete);
            this.data.SaveChanges();

            return athlete;
        }

        protected Exercise SeedExercise(
            int id = 1,
            string name = "Squat",
            string? athleteId = null,
            bool isDeleted = false)
        {
            var exercise = new Exercise
            {
                Id = id,
                Name = name,
                Category = ExerciseCategory.Powerlifting,
                ImageUrl = "https://example.com/img.jpg",
                Description = "A compound lower body exercise",
                IsForBeginners = false,
                AthleteId = athleteId,
                IsDeleted = isDeleted
            };

            this.data.Add(exercise);
            this.data.SaveChanges();

            return exercise;
        }

        protected Achievement SeedAchievement(
            string athleteId,
            int exerciseId,
            int repetitions = 5,
            double weightLifted = 100.0,
            double oneRepMax = 117.46)
        {
            var achievement = new Achievement
            {
                AthleteId = athleteId,
                ExerciseId = exerciseId,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = weightLifted,
                Repetitions = repetitions,
                OneRepMax = oneRepMax
            };

            this.data.Add(achievement);
            this.data.SaveChanges();

            return achievement;
        }

        protected Membership SeedMembership(
            string athleteId,
            bool isMonthly = false,
            bool isPending = true,
            int workoutsLeft = 10,
            int totalWorkoutsCount = 10,
            DateTime? endDate = null)
        {
            var membership = new Membership
            {
                AthleteId = athleteId,
                IsMonthly = isMonthly,
                IsPending = isPending,
                StartDate = DateTime.UtcNow,
                EndDate = endDate,
                WorkoutsLeft = workoutsLeft,
                TotalWorkoutsCount = totalWorkoutsCount,
                Price = 50
            };

            this.data.Add(membership);
            this.data.SaveChanges();

            return membership;
        }

        protected Workout SeedWorkout(
            int id = 1,
            string title = "Morning CrossFit",
            int maxParticipants = 10,
            int currentParticipants = 0,
            DateTime? dateTime = null)
        {
            var workout = new Workout
            {
                Id = id,
                Title = title,
                DateTime = dateTime ?? DateTime.UtcNow.AddDays(1),
                Category = WorkoutCategory.CrossFit,
                MaxParticipants = maxParticipants,
                CurrentParticipants = currentParticipants,
                IsForBeginners = false
            };

            this.data.Add(workout);
            this.data.SaveChanges();

            return workout;
        }
    }
}
