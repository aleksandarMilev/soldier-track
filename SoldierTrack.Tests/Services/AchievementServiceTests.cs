//namespace SoldierTrack.Tests.Services
//{
//    using AutoMapper;
//    using FluentAssertions;
//    using Microsoft.EntityFrameworkCore;
//    using Moq;
//    using SoldierTrack.Data;
//    using SoldierTrack.Data.Models;
//    using SoldierTrack.Services.Achievement;
//    using SoldierTrack.Services.Achievement.MapperProfile;
//    using SoldierTrack.Services.Achievement.Models;
//    using Xunit;

//    using static SoldierTrack.Tests.Services.AchievementServiceTests;

//    public class AchievementServiceTests : IClassFixture<TestDatabaseFixture>
//    {
//        public class TestDatabaseFixture : IDisposable
//        {
//            private readonly DbContextOptions<ApplicationDbContext> options;

//            public TestDatabaseFixture()
//            {
//                this.options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                    .UseInMemoryDatabase(databaseName: "TestAchievementServiceDatabase")
//                    .Options;

//                this.Data = new ApplicationDbContext(this.options);
//            }

//            public void ResetDb()
//            {
//                this.Data.Database.EnsureDeleted();
//                this.Data = new ApplicationDbContext(this.options);
//            }

//            public ApplicationDbContext Data { get; private set; }

//            public void Dispose() => GC.SuppressFinalize(this);
//        }

//        private readonly TestDatabaseFixture fixture;

//        private AchievementService service;

//        private readonly ApplicationDbContext data;
//        private readonly Mock<IMapper> mockMapper;

//        public AchievementServiceTests(TestDatabaseFixture fixture)
//        {
//            this.fixture = fixture;

//            this.data = fixture.Data;
//            this.mockMapper = new Mock<IMapper>();

//            this.service = new AchievementService(this.data, this.mockMapper.Object);
//        }

//        [Fact]
//        public async Task CreateAsync_ShouldThrowInvalidOperationException_IfModelExerciseIdIsInvalid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbAsync();

//            var model = new AchievementServiceModel()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 1,
//                ExerciseId = 241421421
//            };

//            var expected = new Achievement()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 1,
//                ExerciseId = 241421421
//            };

//            this.mockMapper
//                .Setup(m => m.Map<Achievement>(It.IsAny<AchievementServiceModel>()))
//                .Returns(expected);

//            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.CreateAsync(model));
//            exception.Message.Should().Be("Exercise not found!");
//        }

//        [Fact]
//        public async Task CreateAsync_ShouldThrowInvalidOperationException_IfModelAthleteIdIsInvalid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbAsync();

//            var model = new AchievementServiceModel()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 4512512,
//                ExerciseId = 1
//            };

//            var expected = new Achievement()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 4512512,
//                ExerciseId = 1
//            };

//            this.mockMapper
//                .Setup(m => m.Map<Achievement>(It.IsAny<AchievementServiceModel>()))
//                .Returns(expected);

//            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.CreateAsync(model));
//            exception.Message.Should().Be("Athlete not found!");
//        }

//        [Fact]
//        public async Task CreateAsync_ShouldCreateTheMembership()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbAsync();

//            var model = new AchievementServiceModel()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 1,
//                ExerciseId = 1
//            };

//            var expected = new Achievement()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 1,
//                ExerciseId = 1
//            };

//            this.mockMapper
//                .Setup(m => m.Map<Achievement>(It.IsAny<AchievementServiceModel>()))
//                .Returns(expected);
//            await this.service.CreateAsync(model);

//            var result = this.data.Achievements.First();
//            result.Id.Should().Be(1);
//            result.WeightLifted.Should().Be(200);
//            result.AthleteId.Should().Be(1);
//            result.ExerciseId.Should().Be(1);
//        }

//        [Fact]
//        public async Task EditAsync_ShouldThrowInvalidOperationException_IfModelIdIsInvalid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            var model = new AchievementServiceModel()
//            {
//                Id = 24141531,
//                WeightLifted = 200,
//                AthleteId = 1,
//                ExerciseId = 1
//            };

//            this.mockMapper
//               .Setup(m => m.Map(It.IsAny<AchievementServiceModel>(), It.IsAny<Achievement>()))
//               .Callback<AchievementServiceModel, Achievement>((src, dest) =>
//               {
//                   dest.Id = src.Id;
//                   dest.ExerciseId = src.ExerciseId;
//                   dest.AthleteId = src.AthleteId;
//                   dest.WeightLifted = src.WeightLifted;
//               });

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.EditAsync(model));
//        }

//        [Fact]
//        public async Task EditAsync_ShouldEditTheAchievement()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            var model = new AchievementServiceModel()
//            {
//                Id = 1,
//                WeightLifted = 300,
//                AthleteId = 1,
//                ExerciseId = 1
//            };

//            this.mockMapper
//               .Setup(m => m.Map(It.IsAny<AchievementServiceModel>(), It.IsAny<Achievement>()))
//               .Callback<AchievementServiceModel, Achievement>((src, dest) =>
//               {
//                   dest.Id = src.Id;
//                   dest.ExerciseId = src.ExerciseId;
//                   dest.AthleteId = src.AthleteId;
//                   dest.WeightLifted = src.WeightLifted;
//               });

//            await this.service.EditAsync(model);

//            var result = this.data.Achievements.First();
//            result.WeightLifted.Should().Be(300);
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldThrowInvalidOperationException_IfModelIdIsInvalid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.DeleteAsync(3252536));
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldDeleteTheAchievement()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            await this.service.DeleteAsync(1);

//            var result = this.data.Achievements.FirstOrDefault();
//            result.Should().BeNull();
//        }

//        [Fact]
//        public async Task AcheivementIsAlreadyAdded_ShouldRetrunFalse_IfAchievementNotFound()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            var result = await this.service.AchievementIsAlreadyAddedAsync(2, 3);

//            result.Should().BeFalse();
//        }

//        [Fact]
//        public async Task AcheivementIsAlreadyAdded_ShouldRetrunTrue_IfAchievementIsFound()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            var result = await this.service.AchievementIsAlreadyAddedAsync(1, 1);

//            result.Should().BeTrue();
//        }

//        [Fact]
//        public async Task GetByIdAsync_ShouldReturnNull_IfAchievementNotFound() 
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AchievementProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new AchievementService(this.data, realMapper);

//            var result = await this.service.GetByIdAsync(12421);
//            result.Should().BeNull();
//        }

//        [Fact]
//        public async Task GetByIdAsync_ShouldReturnTheAchievement()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithAchievementAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AchievementProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new AchievementService(this.data, realMapper);

//            var result = await this.service.GetByIdAsync(1);
//            result.Should().NotBeNull();
//            result.ExerciseName.Should().Be("Snatch");
//            result.AthleteId.Should().Be(1);
//        }

//        [Fact]
//        public async Task GetAllByAthleteIdAsync_ShouldReturnEmptyCollection_IfAhtleteHasNotAchievements()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithManyAchievementsAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AchievementProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new AchievementService(this.data, realMapper);

//            var result = await this.service.GetAllByAthleteIdAsync(3);
//            result.Count().Should().Be(0);
//        }

//        [Fact]
//        public async Task GetAllByAthleteIdAsync_ShouldReturnTheCorrectAchievements()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDbWithManyAchievementsAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AchievementProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new AchievementService(this.data, realMapper);

//            var result = await this.service.GetAllByAthleteIdAsync(2);
//            result.Count().Should().Be(2);
//            result.First().Id.Should().Be(2);
//            result.Last().Id.Should().Be(3);
//        }

//        private async Task SeedDbAsync()
//        {
//            var athlete = new Athlete()
//            {
//                Id = 1,
//                FirstName = "test-name",
//                LastName = "test-name",
//                PhoneNumber = "0000000000",
//                Email = "test@mail.com,",
//                UserId = "test-userid",
//                MembershipId = null
//            };

//            var exercise = new Exercise()
//            {
//                Id = 1,
//                Name = "Snatch"
//            };

//            this.data.Add(athlete);
//            this.data.Add(exercise);
//            await this.data.SaveChangesAsync();
//        }

//        private async Task SeedDbWithAchievementAsync()
//        {
//            await this.SeedDbAsync();
//            var achievement = new Achievement()
//            {
//                Id = 1,
//                WeightLifted = 200,
//                AthleteId = 1,
//                ExerciseId = 1
//            };

//            this.data.Add(achievement);
//            await this.data.SaveChangesAsync();
//        }

//        private async Task SeedDbWithManyAchievementsAsync()
//        {
//            await this.SeedDbWithAchievementAsync();

//            var exercise2 = new Exercise()
//            {
//                Id = 2,
//                Name = "Clean"
//            };

//            var exercise3 = new Exercise()
//            {
//                Id = 3,
//                Name = "FronSquat"
//            };

//            var athlete2 = new Athlete()
//            {
//                Id = 2,
//                FirstName = "test2-name",
//                LastName = "test2-name",
//                PhoneNumber = "2000000000",
//                Email = "test2@mail.com,",
//                UserId = "test2-userid",
//                MembershipId = null
//            };

//            var athlete3 = new Athlete()
//            {
//                Id = 3,
//                FirstName = "test3-name",
//                LastName = "test3-name",
//                PhoneNumber = "3000000000",
//                Email = "test3@mail.com,",
//                UserId = "test3-userid",
//                MembershipId = null
//            };

//            var achievement2 = new Achievement()
//            {
//                Id = 2,
//                WeightLifted = 150,
//                AthleteId = 2,
//                ExerciseId = 2
//            };

//            var achievement3 = new Achievement()
//            {
//                Id = 3,
//                WeightLifted = 160,
//                AthleteId = 2,
//                ExerciseId = 3
//            };

//            this.data.AddRange(exercise2, exercise3);
//            this.data.AddRange(athlete2, athlete3);
//            this.data.AddRange(achievement2, achievement3);

//            await this.data.SaveChangesAsync();
//        }
//    }
//}
