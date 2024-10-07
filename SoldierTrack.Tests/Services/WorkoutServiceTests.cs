//namespace SoldierTrack.Tests.Services
//{
//    using AutoMapper;
//    using FluentAssertions;
//    using Microsoft.EntityFrameworkCore;
//    using Moq;
//    using SoldierTrack.Data;
//    using SoldierTrack.Data.Models;
//    using SoldierTrack.Services.Workout;
//    using SoldierTrack.Services.Workout.MapperProfile;
//    using SoldierTrack.Services.Workout.Models;
//    using Xunit;

//    using static SoldierTrack.Tests.Services.WorkoutServiceTests;

//    public class WorkoutServiceTests : IClassFixture<TestDatabaseFixture>
//    {
//        public class TestDatabaseFixture : IDisposable
//        {
//            private readonly DbContextOptions<ApplicationDbContext> options;

//            public TestDatabaseFixture()
//            {
//                this.options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                    .UseInMemoryDatabase(databaseName: "TestWorkoutServiceDatabase")
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

//        private WorkoutService service;

//        private readonly ApplicationDbContext data;
//        private readonly Mock<IMapper> mockMapper;

//        public WorkoutServiceTests(TestDatabaseFixture fixture)
//        {
//            this.fixture = fixture;

//            this.data = fixture.Data;
//            this.mockMapper = new Mock<IMapper>();

//            this.service = new WorkoutService(this.data, this.mockMapper.Object);
//        }

//        [Fact]
//        public async Task CreateAsync_ShouldThrowInvalidOperataionException_IfModelCategoryIdIsNotValid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedCategories();

//            var model = new WorkoutServiceModel()
//            {
//                Title = "test1",
//                Date = DateTime.UtcNow,
//                Time = TimeSpan.FromMinutes(60),
//                BriefDescription = "briefdescr",
//                FullDescription = "fulldescr",
//                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                CategoryId = 21421,
//                IsForBeginners = true,
//                MaxParticipants = 15,
//            };

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.CreateAsync(model));
//        }

//        [Fact]
//        public async Task CreateAsync_ShouldCreateTheWorkout()
//        {
//            this.fixture.ResetDb();
//            await this.SeedCategories();

//            var model = new WorkoutServiceModel()
//            {
//                Title = "test1",
//                Date = DateTime.UtcNow,
//                Time = TimeSpan.FromMinutes(60),
//                BriefDescription = "briefdescr",
//                FullDescription = "fulldescr",
//                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                CategoryId = 2,
//                IsForBeginners = true,
//                MaxParticipants = 15,
//            };

//            var expected = new Workout()
//            {
//                Title = "test1",
//                Date = DateTime.UtcNow,
//                Time = TimeSpan.FromMinutes(60),
//                BriefDescription = "briefdescr",
//                FullDescription = "fulldescr",
//                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                CategoryId = 2,
//                IsForBeginners = true,
//                MaxParticipants = 15,
//            };

//            this.mockMapper
//                .Setup(m => m.Map<Workout>(model))
//                .Returns(expected);

//            await this.service.CreateAsync(model);

//            var resilt = await this.data.Workouts.FirstAsync();
//            resilt.Should().NotBeNull();
//            resilt.Title.Should().Be("test1");
//            resilt.MaxParticipants.Should().Be(15);
//            resilt.CategoryId.Should().Be(2);
//        }

//        [Fact]
//        public async Task EditAsync_ShouldThrowInvalidOperationException_IfModelCartegoryDoesNotExists()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var editModel = new EditWorkoutServiceModel()
//            {
//                Id = 1,
//                Title = "test1",
//                Date = DateTime.UtcNow,
//                Time = TimeSpan.FromMinutes(60),
//                BriefDescription = "briefdescr",
//                FullDescription = "fulldescr",
//                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                CategoryId = 1241,
//                IsForBeginners = true,
//                MaxParticipants = 15,
//                CurrentParticipants = 0,
//                CategoryName = "TestCat1",
//            };

//            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.EditAsync(editModel));
//            exception.Message.Should().Be("Category not found!");
//        }

//        [Fact]
//        public async Task EditAsync_ShouldThrowInvalidOperationException_IfModelIdIsNotValid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var editModel = new EditWorkoutServiceModel()
//            {
//                Id = 2412134,
//                Title = "test1",
//                Date = DateTime.UtcNow,
//                Time = TimeSpan.FromMinutes(60),
//                BriefDescription = "briefdescr",
//                FullDescription = "fulldescr",
//                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                CategoryId = 2,
//                IsForBeginners = true,
//                MaxParticipants = 15,
//                CurrentParticipants = 0,
//                CategoryName = "TestCat1",

//            };

//            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.EditAsync(editModel));
//            exception.Message.Should().Be("Workout not found!");
//        }

//        [Fact]
//        public async Task EditAsync_ShouldEditTheModel()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var editModel = new EditWorkoutServiceModel()
//            {
//                Id = 1,
//                Title = "test1edited",
//                Date = DateTime.UtcNow,
//                Time = TimeSpan.FromMinutes(60),
//                BriefDescription = "briefdescr",
//                FullDescription = "fulldescr",
//                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                CategoryId = 1,
//                IsForBeginners = true,
//                MaxParticipants = 15,
//                CurrentParticipants = 0,
//                CategoryName = "TestCat1",
//            };

//            this.mockMapper
//                .Setup(m => m.Map(editModel, It.IsAny<Workout>()))
//                .Callback<EditWorkoutServiceModel, Workout>((src, dest) =>
//                {
//                    dest.Title = src.Title;
//                    dest.Date = src.Date;
//                    dest.BriefDescription = src.BriefDescription;
//                    dest.FullDescription = src.FullDescription;
//                    dest.ImageUrl = src.ImageUrl;
//                    dest.CategoryId = src.CategoryId;
//                    dest.IsForBeginners = src.IsForBeginners;
//                    dest.CurrentParticipants = src.CurrentParticipants;
//                    dest.MaxParticipants = src.MaxParticipants;
//                });

//            await this.service.EditAsync(editModel);

//            var result = await this.data.Workouts.FirstAsync(w => w.Id == editModel.Id);
//            result.Title.Should().Be(editModel.Title);
//            result.BriefDescription.Should().Be(editModel.BriefDescription);
//            result.FullDescription.Should().Be(editModel.FullDescription);
//            result.ImageUrl.Should().Be(editModel.ImageUrl);
//            result.CategoryId.Should().Be(editModel.CategoryId);
//            result.IsForBeginners.Should().Be(editModel.IsForBeginners);
//            result.MaxParticipants.Should().Be(editModel.MaxParticipants);
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldThrowInvalidOperationException_IfModelIdIsNotValid()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.DeleteAsync(21421421));
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldSoftDeleteTheWorkout()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            await this.service.DeleteAsync(1);

//            var result = await this.data.Workouts.FirstAsync(w => w.Id == 1);
//            result.IsDeleted.Should().BeTrue();
//        }

//        [Fact]
//        public async Task IsAnotherWorkoutScheduledAtThisDateAndTimeAsync_ShouldRetrunTrue_IfThereIsSuchWorkout()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();
//            var workoutEntity = await this.data.Workouts.FirstAsync(w => w.Id == 1);

//            var result = await this.service.AnotherWorkoutExistsAtThisDateAndTimeAsync(workoutEntity.Date, workoutEntity.Time);

//            result.Should().BeTrue();
//        }

//        [Fact]
//        public async Task IsAnotherWorkoutScheduledAtThisDateAndTimeAsync_ShouldRetrunFalse_IfThereIsNotSuchWorkout()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var result = await this.service.AnotherWorkoutExistsAtThisDateAndTimeAsync(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(60));

//            result.Should().BeFalse();
//        }

//        [Fact]
//        public async Task GetByIdAsync_ShouldRetrunNull_IfThereIfWorkoutIsNotFound()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new WorkoutProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new WorkoutService(this.data, realMapper);

//            var result = await this.service.GetEditModelByIdAsync(23141);
//            result.Should().BeNull();
//        }

//        [Fact]
//        public async Task GetByIdAsync_ShouldRetrunTheCorrectModel()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new WorkoutProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new WorkoutService(this.data, realMapper);

//            var result = await this.service.GetEditModelByIdAsync(1);
//            result.Should().NotBeNull();
//            result.Id.Should().Be(1);
//            result.Title.Should().Be("test1");
//        }

//        [Fact]
//        public async Task GetDetailsByIdAsync_ShouldReturnNull_IfWorkoutIsNotFound()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new WorkoutProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new WorkoutService(this.data, realMapper);

//            var result = await this.service.GetDetailsModelByIdAsync(4214221);
//            result.Should().BeNull();
//        }

//        [Fact]
//        public async Task GetDetailsByIdAsync_ShouldReturnTheCorrectModel()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new WorkoutProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new WorkoutService(this.data, realMapper);

//            var result = await this.service.GetDetailsModelByIdAsync(1);
//            result.Should().NotBeNull();
//            result.Id.Should().Be(1);
//            result.Title.Should().Be("test1");
//        }

//        [Fact]
//        public async Task GetAllAsync_ShouldReturnTheCorrectModels_IfSuchExist()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new WorkoutProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new WorkoutService(this.data, realMapper);

//            var expectedDate = this.data.Workouts.First().Date;

//            var result = await this.service.GetAllAsync(expectedDate, pageIndex: 1, pageSize: 10);

//            result.Should().NotBeNull();
//            result.TotalCount.Should().BeGreaterThan(0);
//            result.PageSize.Should().Be(10); 
//            result.PageIndex.Should().Be(1); 

//            result.Workouts.First().Title.Should().NotBeNullOrEmpty();
//        }

//        [Fact]
//        public async Task GetAllAsync_ShouldReturnZeroModels_IfNothingFound()
//        {
//            this.fixture.ResetDb();
//            await this.SeedWorkoutsWithCategoriesAsync();

//            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new WorkoutProfile()));
//            var realMapper = mapperConfig.CreateMapper();
//            this.service = new WorkoutService(this.data, realMapper);

//            var expectedDate = DateTime.UtcNow.AddDays(50);

//            var result = await this.service.GetAllAsync(expectedDate, pageIndex: 1, pageSize: 10);

//            result.Should().NotBeNull();
//            result.TotalCount.Should().Be(0);
//            result.PageSize.Should().Be(10);
//            result.PageIndex.Should().Be(1);
//            result.Workouts.Count().Should().Be(0);
//        }

//        private async Task SeedCategories()
//        {
//            var categories = new Category[]
//            {
//                new(){ Name = "TestCat1" },
//                new(){ Name = "TestCat2" },
//                new(){ Name = "TestCat3" },
//                new(){ Name = "TestCat4" },
//                new(){ Name = "TestCat5" },
//            };

//            this.data.AddRange(categories);
//            await this.data.SaveChangesAsync();
//        }

//        private async Task SeedWorkoutsWithCategoriesAsync()
//        {
//            await this.SeedCategories();

//            var workouts = new Workout[]
//            {
//                new()
//                {
//                    Title = "test1",
//                    Date = DateTime.UtcNow,
//                    Time = TimeSpan.FromMinutes(60),
//                    BriefDescription = "briefdescr",
//                    FullDescription = "fulldescr",
//                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                    CategoryId = 1,
//                    IsForBeginners = true,
//                    MaxParticipants = 15,
//                },

//                new()
//                {
//                    Title = "test2",
//                    Date = DateTime.UtcNow.AddDays(1),
//                    Time = TimeSpan.FromMinutes(60),
//                    BriefDescription = null,
//                    FullDescription = "fulldescr",
//                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                    CategoryId = 2,
//                    IsForBeginners = false,
//                    MaxParticipants = 5,
//                },

//                new()
//                {
//                    Title = "test3",
//                    Date = DateTime.UtcNow.AddDays(2),
//                    Time = TimeSpan.FromMinutes(60),
//                    BriefDescription = "briefdescr",
//                    FullDescription = null,
//                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSVHZh9LmC_ScCXjK7rEmUHgcc3tHQJyBMa8Q&s",
//                    CategoryId = 3,
//                    IsForBeginners = true,
//                    MaxParticipants = 10,
//                }
//            };

//            this.data.AddRange(workouts);
//            await this.data.SaveChangesAsync();
//        }
//    }
//}
