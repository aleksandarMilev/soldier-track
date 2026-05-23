namespace SoldierTrack.Tests.Achievement
{
    using Common;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Services.Achievement;
    using Services.Achievement.Models;
    using Services.Exercise;
    using Xunit;

    public class AchievementServiceTests : ServiceTestBase
    {
        private readonly Mock<IExerciseService> exerciseServiceMock;
        private readonly AchievementService service;

        public AchievementServiceTests()
        {
            this.exerciseServiceMock = new Mock<IExerciseService>();

            this.service = new AchievementService(
                this.data,
                new Lazy<IExerciseService>(() => this.exerciseServiceMock.Object),
                new Mock<ILogger<AchievementService>>().Object);
        }

        [Fact]
        public async Task Create_WithValidData_PersistsAchievementWithCorrectAthleteId()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();

            this.exerciseServiceMock
                .Setup(s => s.ExistsById(exercise.Id))
                .ReturnsAsync(true);

            var model = new AchievementServiceModel
            {
                ExerciseId = exercise.Id,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = 100,
                Repetitions = 5
            };

            await this.service.Create(model, athlete.Id);

            var saved = this.data.Achievements.Single();
            saved.AthleteId.Should().Be(athlete.Id);
            saved.ExerciseId.Should().Be(exercise.Id);
        }

        [Fact]
        public async Task Create_WithTenOrFewerReps_CalculatesOneRepMaxWithPowerFormula()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();

            this.exerciseServiceMock
                .Setup(s => s.ExistsById(exercise.Id))
                .ReturnsAsync(true);

            var model = new AchievementServiceModel
            {
                ExerciseId = exercise.Id,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = 100,
                Repetitions = 5
            };

            await this.service.Create(model, athlete.Id);

            var expected = 100.0 * Math.Pow(5, 0.1);
            this.data
                .Achievements
                .Single()
                .OneRepMax
                .Should()
                .BeApproximately(expected, 0.001);
        }

        [Fact]
        public async Task Create_WithMoreThanTenReps_CalculatesOneRepMaxWithLinearFormula()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();

            this.exerciseServiceMock
                .Setup(s => s.ExistsById(exercise.Id))
                .ReturnsAsync(true);

            var model = new AchievementServiceModel
            {
                ExerciseId = exercise.Id,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = 100,
                Repetitions = 12
            };

            await this.service.Create(model, athlete.Id);

            var expected = 100.0 * (1 + 0.0333 * 12);
            this.data
                .Achievements
                .Single()
                .OneRepMax
                .Should()
                .BeApproximately(expected, 0.001);
        }

        [Fact]
        public async Task Create_WhenAchievementAlreadyExistsForSameExercise_ThrowsInvalidOperationException()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();
            this.SeedAchievement(athlete.Id, exercise.Id);

            var model = new AchievementServiceModel
            {
                ExerciseId = exercise.Id,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = 120,
                Repetitions = 3
            };

            var create = async ()
                => await this.service.Create(model, athlete.Id);

            await create
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("PR already exists for this exercise!");
        }

        [Fact]
        public async Task Create_WithInvalidExerciseId_ThrowsInvalidOperationException()
        {
            var athlete = this.SeedAthlete();

            this.exerciseServiceMock
                .Setup(s => s.ExistsById(It.IsAny<int>()))
                .ReturnsAsync(false);

            var model = new AchievementServiceModel
            {
                ExerciseId = 999,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = 100,
                Repetitions = 5
            };

            var create = async ()
                => await this.service.Create(model, athlete.Id);

            await create
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Invalid exercise!");
        }

        [Fact]
        public async Task Edit_WithValidOwner_UpdatesWeightAndRepsAndRecalculatesOneRepMax()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();
            var achievement = this.SeedAchievement(
                athlete.Id,
                exercise.Id,
                repetitions: 5,
                weightLifted: 100);

            var model = new AchievementServiceModel
            {
                Id = achievement.Id,
                ExerciseId = exercise.Id,
                DateAchieved = achievement.DateAchieved,
                WeightLifted = 140,
                Repetitions = 3
            };

            await this.service.Edit(model, athlete.Id);

            var updated = this.data.Achievements.Single();
            updated.WeightLifted.Should().Be(140);
            updated.Repetitions.Should().Be(3);
            updated
                .OneRepMax
                .Should()
                .BeApproximately(140.0 * Math.Pow(3, 0.1), 0.001);
        }

        [Fact]
        public async Task Edit_WhenAchievementNotFound_ThrowsInvalidOperationException()
        {
            var athlete = this.SeedAthlete();

            var model = new AchievementServiceModel
            {
                Id = 999,
                ExerciseId = 1,
                DateAchieved = DateTime.UtcNow,
                WeightLifted = 100,
                Repetitions = 5
            };

            var edit = async ()
                => await this.service.Edit(model, athlete.Id);

            await edit
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Achievement not found!");
        }

        [Fact]
        public async Task Edit_WhenCalledByDifferentAthlete_ThrowsInvalidOperationException()
        {
            var owner = this.SeedAthlete("owner-1");
            var other = this.SeedAthlete("other-1", email: "other@example.com");
            var exercise = this.SeedExercise();
            var achievement = this.SeedAchievement(owner.Id, exercise.Id);

            var model = new AchievementServiceModel
            {
                Id = achievement.Id,
                ExerciseId = exercise.Id,
                DateAchieved = achievement.DateAchieved,
                WeightLifted = 150,
                Repetitions = 5
            };

            var edit = async ()
                => await this.service.Edit(model, other.Id);

            await edit
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Unauthorized operation!");
        }

        [Fact]
        public async Task Delete_WithValidOwner_RemovesAchievementFromDatabase()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();
            var achievement = this.SeedAchievement(athlete.Id, exercise.Id);

            await this.service.Delete(achievement.Id, athlete.Id);

            this.data.Achievements.Should().BeEmpty();
        }

        [Fact]
        public async Task Delete_WhenAchievementNotFound_ThrowsInvalidOperationException()
        {
            var delete = async () => await this.service.Delete(999, "some-athlete");

            await delete
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Achievement not found!");
        }

        [Fact]
        public async Task Delete_WhenCalledByDifferentAthlete_ThrowsInvalidOperationException()
        {
            var owner = this.SeedAthlete("owner-1");
            var other = this.SeedAthlete("other-1", email: "other@example.com");
            var exercise = this.SeedExercise();
            var achievement = this.SeedAchievement(owner.Id, exercise.Id);

            var delete = async ()
                => await this.service.Delete(achievement.Id, other.Id);

            await delete
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Unauthorized operation!");
        }

        [Fact]
        public async Task GetAchievementId_WhenExists_ReturnsId()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();
            var achievement = this.SeedAchievement(athlete.Id, exercise.Id);

            var result = await this.service.GetAchievementId(
                athlete.Id,
                exercise.Id);

            result.Should().Be(achievement.Id);
        }

        [Fact]
        public async Task GetAchievementId_WhenNotExists_ReturnsNull()
        {
            var result = await this.service.GetAchievementId("no-athlete", 999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task AchievementIsAlreadyAdded_WhenExists_ReturnsTrue()
        {
            var athlete = this.SeedAthlete();
            var exercise = this.SeedExercise();
            this.SeedAchievement(athlete.Id, exercise.Id);

            var result = await this.service.AchievementIsAlreadyAdded(
                exercise.Id,
                athlete.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task AchievementIsAlreadyAdded_WhenNotExists_ReturnsFalse()
        {
            var result = await this.service.AchievementIsAlreadyAdded(
                1,
                "no-athlete");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllByAthleteId_ReturnsPaginatedResultOrderedByExerciseName()
        {
            var athlete = this.SeedAthlete();
            var exerciseA = this.SeedExercise(id: 1, name: "Bench Press");
            var exerciseB = this.SeedExercise(id: 2, name: "Deadlift");
            var exerciseC = this.SeedExercise(id: 3, name: "Squat");

            this.SeedAchievement(athlete.Id, exerciseA.Id);
            this.SeedAchievement(athlete.Id, exerciseB.Id);
            this.SeedAchievement(athlete.Id, exerciseC.Id);

            var result = await this.service.GetAllByAthleteId(
                athlete.Id,
                pageIndex: 1,
                pageSize: 10);

            result.Items.Should().HaveCount(3);
            result
                .Items
                .Select(a => a.ExerciseName)
                .Should()
                .BeInAscendingOrder();
        }

        [Fact]
        public async Task GetRankings_ReturnsTopTenOrderedByOneRepMaxDescending()
        {
            var exercise = this.SeedExercise();

            for (var i = 1; i <= 12; i++)
            {
                var athlete = this.SeedAthlete(
                    id: $"athlete-{i}",
                    email: $"athlete{i}@example.com");

                this.SeedAchievement(
                    athlete.Id,
                    exercise.Id,
                    oneRepMax: i * 10.0);
            }

            var rankings = (await this.service.GetRankings(exercise.Id)).ToList();

            rankings.Should().HaveCount(10);
            rankings.Should().BeInDescendingOrder(r => r.OneRepMax);
        }
    }
}
