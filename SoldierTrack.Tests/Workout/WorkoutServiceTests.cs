namespace SoldierTrack.Tests.Workout
{
    using AutoMapper;
    using Common;
    using Data.Models;
    using Data.Models.Enums;
    using FluentAssertions;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Services.Athlete;
    using Services.Membership;
    using Services.Workout;
    using Services.Workout.MapperProfile;
    using Services.Workout.Models;
    using Xunit;

    public class WorkoutServiceTests : ServiceTestBase
    {
        private readonly Mock<IMembershipService> membershipServiceMock;
        private readonly Mock<IAthleteService> athleteServiceMock;
        private readonly WorkoutService service;

        public WorkoutServiceTests()
        {
            this.membershipServiceMock = new Mock<IMembershipService>();
            this.athleteServiceMock = new Mock<IAthleteService>();

            var mapper = new MapperConfiguration(
                    config => config.AddProfile<WorkoutProfile>(),
                    NullLoggerFactory.Instance)
                .CreateMapper();

            this.service = new WorkoutService(
                this.data,
                this.membershipServiceMock.Object,
                this.athleteServiceMock.Object,
                mapper);
        }

        [Fact]
        public async Task WorkoutIsFull_WhenCurrentEqualsMax_ReturnsTrue()
        {
            this.SeedWorkout(
                maxParticipants: 10,
                currentParticipants: 10);

            var result = await this.service.WorkoutIsFull(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task WorkoutIsFull_WhenCurrentIsLessThanMax_ReturnsFalse()
        {
            this.SeedWorkout(
                maxParticipants: 10,
                currentParticipants: 5);

            var result = await this.service.WorkoutIsFull(1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task WorkoutIsFull_WhenWorkoutNotFound_ThrowsInvalidOperationException()
        {
            var workoutIsFull = async ()
                => await this.service.WorkoutIsFull(999);

            await workoutIsFull
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Workout is not found!");
        }

        [Fact]
        public async Task AnotherWorkoutExistsAtThisDateAndTime_WhenSlotIsTaken_ReturnsTrue()
        {
            var futureDate = DateTime.Now.AddDays(1).Date;
            var time = new TimeSpan(10, 0, 0);
            var utcDateTime = (futureDate + time).ToUniversalTime();

            this.SeedWorkout(dateTime: utcDateTime);

            var result = await this.service.AnotherWorkoutExistsAtThisDateAndTime(
                futureDate,
                time);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task AnotherWorkoutExistsAtThisDateAndTime_WhenSlotIsFree_ReturnsFalse()
        {
            var result = await this.service.AnotherWorkoutExistsAtThisDateAndTime(
                DateTime.Now.AddDays(5).Date,
                new TimeSpan(14, 0, 0));

            result.Should().BeFalse();
        }

        [Fact]
        public async Task AnotherWorkoutExistsAtThisDateAndTime_WhenCheckingOwnId_ReturnsFalse()
        {
            var futureDate = DateTime.Now.AddDays(1).Date;
            var time = new TimeSpan(10, 0, 0);
            var utcDateTime = (futureDate + time).ToUniversalTime();

            var workout = this.SeedWorkout(
                id: 1,
                dateTime: utcDateTime);

            var result = await this.service.AnotherWorkoutExistsAtThisDateAndTime(
                futureDate,
                time,
                id: workout.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task Create_WithValidModel_PersistsWorkoutWithUtcDateTime()
        {
            var localDate = DateTime.Now.AddDays(2).Date;
            var localTime = new TimeSpan(9, 30, 0);

            var model = new WorkoutServiceModel
            {
                Title = "Evening HIIT",
                Date = localDate,
                Time = localTime,
                Category = WorkoutCategory.CrossFit,
                MaxParticipants = 12,
                IsForBeginners = true
            };

            var id = await this.service.Create(model);

            var saved = this.data.Workouts.Single();
            saved.Id.Should().Be(id);
            saved.Title.Should().Be("Evening HIIT");
            saved.DateTime.Kind.Should().Be(DateTimeKind.Utc);
            saved.DateTime.Should().Be((localDate + localTime).ToUniversalTime());
        }

        [Fact]
        public async Task Edit_WithValidModel_UpdatesWorkoutInDatabase()
        {
            this.SeedWorkout(id: 1, title: "Old Title");

            var model = new WorkoutServiceModel
            {
                Id = 1,
                Title = "New Title",
                Date = DateTime.Now.AddDays(1).Date,
                Time = new TimeSpan(10, 0, 0),
                Category = WorkoutCategory.Weightlifting,
                MaxParticipants = 8,
                IsForBeginners = false
            };

            await this.service.Edit(model);

            this
                .data
                .Workouts
                .Single()
                .Title
                .Should()
                .Be("New Title");
        }

        [Fact]
        public async Task Delete_WhenWorkoutHasNoParticipants_SoftDeletesWorkout()
        {
            this.SeedWorkout(id: 1);

            await this.service.Delete(1);

            this
                .data
                .Workouts
                .Single()
                .IsDeleted
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task Delete_WhenWorkoutHasParticipants_RemovesAthleteWorkoutEntriesAndSendsEmails()
        {
            var athlete = this.SeedAthlete();
            var membership = this.SeedMembership(
                athlete.Id,
                isPending: false);

            athlete.MembershipId = membership.Id;
            this.data.SaveChanges();

            var workout = this.SeedWorkout(
                id: 1,
                currentParticipants: 1);

            var mappingObject = new AthleteWorkout()
            {
                AthleteId = athlete.Id,
                WorkoutId = workout.Id
            };

            this.data.Add(mappingObject);
            this.data.SaveChanges();

            this.membershipServiceMock
                .Setup(s => s.UpdateMembershipOnWorkoutDeletion(It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            this.athleteServiceMock
                .Setup(s => s.SendMailOnWorkoutDeletionByAthleteId(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await this.service.Delete(1);

            this.data.AthletesWorkouts.Should().BeEmpty();
            this.data.Workouts.Single().IsDeleted.Should().BeTrue();

            this.athleteServiceMock.Verify(
                s => s.SendMailOnWorkoutDeletionByAthleteId(
                    athlete.Id,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsOnlyFutureWorkoutsOrderedByDate()
        {
            this.SeedWorkout(
                id: 1,
                title: "Future A",
                dateTime: DateTime.UtcNow.AddDays(3));

            this.SeedWorkout(
                id: 2,
                title: "Future B",
                dateTime: DateTime.UtcNow.AddDays(1));

            this.SeedWorkout(
                id: 3,
                title: "Past",
                dateTime: DateTime.UtcNow.AddDays(-1));

            var result = await this.service.GetAll(
                null,
                pageIndex: 1,
                pageSize: 10);

            result.Workouts.Should().HaveCount(2);
            result
                .Workouts
                .Select(w => w.Title)
                .Should()
                .ContainInOrder("Future B", "Future A");
        }
    }
}
