namespace SoldierTrack.Tests.Exercise
{
    using AutoMapper;
    using FluentAssertions;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Services.Achievement;
    using Services.Exercise;
    using Services.Exercise.MapperProfile;
    using Services.Exercise.Models;
    using SoldierTrack.Tests.Common;
    using Xunit;

    public class ExerciseServiceTests : ServiceTestBase
    {
        private readonly ExerciseService service;

        public ExerciseServiceTests()
        {
            var mapper = new MapperConfiguration(
                    config => config.AddProfile<ExerciseProfile>(),
                    NullLoggerFactory.Instance)
                .CreateMapper();

            this.service = new ExerciseService(
                this.data,
                new Lazy<IAchievementService>(() => new Mock<IAchievementService>().Object),
                mapper);
        }

        [Fact]
        public async Task ExistsById_WhenExerciseExists_ReturnsTrue()
        {
            this.SeedExercise(id: 1);

            var result = await this.service.ExistsById(1);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsById_WhenExerciseDoesNotExist_ReturnsFalse()
        {
            var result = await this.service.ExistsById(999);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsById_WhenExerciseIsSoftDeleted_ReturnsFalse()
        {
            this.SeedExercise(id: 1, isDeleted: true);

            var result = await this.service.ExistsById(1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExerciseWithThisNameExists_WhenNameMatches_ReturnsTrue()
        {
            this.SeedExercise(name: "Deadlift");

            var result = await this.service.ExerciseWithThisNameExists("Deadlift");

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExerciseWithThisNameExists_WhenNoMatch_ReturnsFalse()
        {
            this.SeedExercise(name: "Deadlift");

            var result = await this.service.ExerciseWithThisNameExists("Bench Press");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExerciseLimitReached_WhenCountExceedsMax_ReturnsTrue()
        {
            var athlete = this.SeedAthlete();

            for (var i = 1; i <= 51; i++)
            {
                this.SeedExercise(
                    id: i,
                    name: $"Exercise {i}",
                    athleteId: athlete.Id);
            }

            var result = await this.service.ExerciseLimitReached(athlete.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExerciseLimitReached_WhenCountIsAtOrBelowMax_ReturnsFalse()
        {
            var athlete = this.SeedAthlete();

            for (var i = 1; i <= 50; i++)
            {
                this.SeedExercise(
                    id: i,
                    name: $"Exercise {i}",
                    athleteId: athlete.Id);
            }

            var result = await this.service.ExerciseLimitReached(athlete.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetNameById_WhenExerciseExists_ReturnsName()
        {
            this.SeedExercise(id: 1, name: "Overhead Press");

            var name = await this.service.GetNameById(1);

            name.Should().Be("Overhead Press");
        }

        [Fact]
        public async Task GetNameById_WhenExerciseNotFound_ThrowsInvalidOperationException()
        {
            var getNameById = async () => await this.service.GetNameById(999);

            await getNameById
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Exercise is not found!");
        }

        [Fact]
        public async Task Create_WithValidModel_PersistsExerciseAndReturnsId()
        {
            var athlete = this.SeedAthlete();

            var model = new ExerciseServiceModel
            {
                Name = "Romanian Deadlift",
                ImageUrl = "https://example.com/rdl.jpg",
                Description = "A hip-hinge posterior chain exercise",
                Category = "Powerlifting",
                IsForBeginners = false,
                AthleteId = athlete.Id
            };

            var id = await this.service.Create(model);

            var saved = this
                .data
                .Exercises
                .Single(e => e.Name == "Romanian Deadlift");

            saved.Id.Should().Be(id);
            saved.AthleteId.Should().Be(athlete.Id);
        }

        [Fact]
        public async Task Delete_WhenCalledByOwner_SoftDeletesExercise()
        {
            var athlete = this.SeedAthlete();
            this.SeedExercise(id: 1, athleteId: athlete.Id);

            await this.service.Delete(
                1,
                athlete.Id,
                userIsAdmin: false);

            this
                .data
                .Exercises
                .Single()
                .IsDeleted
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task Delete_WhenCalledByAdmin_SoftDeletesGlobalExercise()
        {
            this.SeedExercise(id: 1, athleteId: null);

            await this.service.Delete(
                1,
                "any-athlete",
                userIsAdmin: true);

            this
                .data
                .Exercises
                .Single()
                .IsDeleted
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task Delete_WhenAthleteDoesNotOwnExercise_ThrowsInvalidOperationException()
        {
            var owner = this.SeedAthlete("owner-1");
            var other = this.SeedAthlete("other-1", email: "other@example.com");
            this.SeedExercise(id: 1, athleteId: owner.Id);

            var delete = async ()
                => await this.service.Delete(
                    1,
                    other.Id,
                    userIsAdmin: false);

            await delete
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Unauthorized operation!");
        }

        [Fact]
        public async Task Delete_WhenAthleteTriesToDeleteGlobalExercise_ThrowsInvalidOperationException()
        {
            var athlete = this.SeedAthlete();
            this.SeedExercise(id: 1, athleteId: null);

            var delete = async ()
                => await this.service.Delete(
                    1,
                    athlete.Id,
                    userIsAdmin: false);

            await delete
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Unauthorized operation!");
        }

        [Fact]
        public async Task Delete_WhenExerciseNotFound_ThrowsInvalidOperationException()
        {
            var delete = async ()
                => await this.service.Delete(
                    999,
                    "any-athlete",
                    userIsAdmin: false);

            await delete
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Exercise not found!");
        }
    }
}
