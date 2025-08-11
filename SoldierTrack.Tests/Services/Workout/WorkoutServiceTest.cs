namespace SoldierTrack.Tests.Services.Workout
{
    using AutoMapper;
    using FluentAssertions;
    using Moq;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Data.Models.Enums;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.Models;
    using Xunit;

    public class WorkoutServiceTest : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture fixture;
        private readonly Mock<IMembershipService> mockMembershipService;
        private readonly Mock<IAthleteService> mockAthleteService;
        private readonly WorkoutService workoutService;
        private readonly IMapper mapper;

        public WorkoutServiceTest(TestDatabaseFixture fixture)
        {
            this.fixture = fixture;

            this.mockMembershipService = new Mock<IMembershipService>();
            this.mockAthleteService = new Mock<IAthleteService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Workout, WorkoutServiceModel>().ReverseMap();
                cfg.CreateMap<Workout, WorkoutDetailsServiceModel>();
            });

            this.mapper = config.CreateMapper();

            this.workoutService = new WorkoutService(
                this.fixture.Data,
                this.mockMembershipService.Object,
                this.mockAthleteService.Object,
                this.mapper
            );
        }

        private void ResetDatabase()
        {
            this.fixture.ResetDatabase();
            SeedWorkouts();
        }

        private void SeedWorkouts()
        {
            if (this.fixture.Data.Workouts.Any())
            {
                return;
            }

            var workouts = new Workout[]
            {
                new()
                {
                    Id = 1,
                    Title = "Murph",
                    DateTime = DateTime.UtcNow.AddDays(1),
                    BriefDescription = "Murph is a grueling Hero WOD...",
                    FullDescription = "Murph is one of the most iconic Hero WODs...",
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
                    DateTime = DateTime.UtcNow.AddDays(2),
                    BriefDescription = "Fran is a fast and intense CrossFit workout...",
                    FullDescription = "Fran is a classic CrossFit benchmark workout...",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSU2qyjxN9nLMueVzxB79jBW3AUwKmUWQFzDQ&s",
                    Category = WorkoutCategory.CrossFit,
                    IsForBeginners = true,
                    MaxParticipants = 12,
                    CurrentParticipants = 0,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            this.fixture.Data.Workouts.AddRange(workouts);
            this.fixture.Data.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedWorkouts_WhenWorkoutsExist()
        {
            this.ResetDatabase();

            var result = await this.workoutService.GetAll(null, 1, 2);

            result.Should().NotBeNull();
            result.Workouts.Should().HaveCount(2);
            result.TotalPages.Should().Be(2);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateWorkout_WhenValidModelIsProvided()
        {
            this.ResetDatabase();

            var model = new WorkoutServiceModel()
            {
                Title = "New Workout",
                Date = DateTime.UtcNow.AddDays(1).Date,
                Time = TimeSpan.FromHours(9),
                BriefDescription = "A new workout",
                FullDescription = "A detailed description of the new workout",
                Category = WorkoutCategory.CrossFit,
                MaxParticipants = 10,
                CurrentParticipants = 0
            };

            var result = await this.workoutService.Create(model);

            result.Should().BeGreaterThan(0);

            var createdWorkout = await this.fixture.Data.Workouts.FindAsync(result);
            createdWorkout.Should().NotBeNull();
            createdWorkout?.Title.Should().Be(model.Title);
        }

        [Fact]
        public async Task EditAsync_ShouldUpdateWorkout_WhenValidModelIsProvided()
        {
            this.ResetDatabase();

            var workout = this.fixture.Data.Workouts.First();

            var model = new WorkoutServiceModel()
            {
                Id = workout.Id,
                Title = "Updated Workout Title",
                Date = DateTime.UtcNow.AddDays(2).Date,
                Time = TimeSpan.FromHours(10),
                BriefDescription = "Updated brief description",
                FullDescription = "Updated full description",
                Category = WorkoutCategory.CrossFit,
                MaxParticipants = 20,
                CurrentParticipants = 0
            };

            var result = await this.workoutService.Edit(model);

            result.Should().Be(workout.Id);

            var updatedWorkout = await this.fixture.Data.Workouts.FindAsync(workout.Id);
            updatedWorkout.Should().NotBeNull();
            updatedWorkout?.Title.Should().Be(model.Title);
            updatedWorkout?.DateTime.Date.Should().Be(model.Date);
            updatedWorkout?.DateTime.TimeOfDay.Should().Be(model.Time);
        }

        [Fact]
        public async Task EditAsync_ShouldThrowException_WhenWorkoutNotFound()
        {
            this.ResetDatabase();

            var model = new WorkoutServiceModel()
            {
                Id = 999,
                Title = "Non-existent Workout",
                Date = DateTime.UtcNow.AddDays(2).Date,
                Time = TimeSpan.FromHours(10),
                BriefDescription = "Invalid workout",
                FullDescription = "Invalid workout description",
                Category = WorkoutCategory.CrossFit,
                MaxParticipants = 20,
                CurrentParticipants = 0
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.workoutService.Edit(model));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteWorkout_WhenValidIdIsProvided()
        {
            this.ResetDatabase();

            var workout = this.fixture.Data.Workouts.First();

            await this.workoutService.Delete(workout.Id);

            var deletedWorkout = await this.fixture.Data.Workouts.FindAsync(workout.Id);
            deletedWorkout?.IsDeleted.Should().BeTrue();
        }
    }
}
