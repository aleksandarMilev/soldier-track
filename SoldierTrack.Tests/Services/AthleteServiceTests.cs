namespace SoldierTrack.Tests.Services
{
    using AutoMapper;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Athlete.MapperProfile;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Exceptions;
    using SoldierTrack.Services.Workout.Exceptions;
    using Xunit;

    using static SoldierTrack.Tests.Services.AthleteServiceTests;

    public class AthleteServiceTests : IClassFixture<TestDatabaseFixture>
    {
        public class TestDatabaseFixture : IDisposable
        {
            private readonly DbContextOptions<ApplicationDbContext> options;

            public TestDatabaseFixture()
            {
                this.options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestAthleteServiceDatabase")
                    .Options;

                this.Data = new ApplicationDbContext(this.options);
            }

            public void ResetDb()
            {
                this.Data.Database.EnsureDeleted();
                this.Data = new ApplicationDbContext(this.options);
            }

            public ApplicationDbContext Data { get; private set; }

            public void Dispose() => GC.SuppressFinalize(this);
        }

        private readonly TestDatabaseFixture fixture;

        private AthleteService service;

        private readonly ApplicationDbContext data;
        private readonly Mock<IMembershipService> mockMembershipService;
        private readonly Mock<IMapper> mockMapper;

        public AthleteServiceTests(TestDatabaseFixture fixture)
        {
            this.fixture = fixture;

            this.data = fixture.Data;
            this.mockMembershipService = new Mock<IMembershipService>();
            this.mockMapper = new Mock<IMapper>();

            this.service = new AthleteService(this.data, this.mockMembershipService.Object, this.mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreate_TheExpectedEntity()
        {
            this.fixture.ResetDb();

            var athleteCreateModel = new AthleteServiceModel()
            {
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                UserId = "test-userid"
            };

            var expected = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                UserId = "test-userid",
                MembershipId = null,
                Email = null
            };

            this.mockMapper
                .Setup(m => m.Map<Athlete>(It.IsAny<AthleteServiceModel>()))
                .Returns(expected);

            await this.service.CreateAsync(athleteCreateModel);

            var actual = await this.data.Athletes.FirstAsync();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task EditAsync_ShouldThrowInvalidOperationException_IfModelIdIsNotValid()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var athleteEditModel = new EditAthleteServiceModel()
            {
                Id = 231241421,
                FirstName = "test1 edited",
                LastName = "test1",
                PhoneNumber = "0000000000",
                UserId = "test-user-id1"
            };

            var expected = new Athlete()
            {
                Id = 1,
                FirstName = "test1 edited",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            this.mockMapper
                .Setup(m => m.Map(athleteEditModel, It.IsAny<Athlete>()))
                .Callback<EditAthleteServiceModel, Athlete>((src, dest) =>
                {
                    dest.FirstName = src.FirstName;
                    dest.LastName = src.LastName;
                    dest.PhoneNumber = src.PhoneNumber;
                    dest.UserId = src.UserId;
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.EditAsync(athleteEditModel));
        }

        [Fact]
        public async Task EditAsync_ShouldEdit_TheEntity()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var athleteEditModel = new EditAthleteServiceModel()
            {
                Id = 1,
                FirstName = "test1 edited",
                LastName = "test1",
                PhoneNumber = "0000000000",
                UserId = "test-user-id1"
            };

            var expected = new Athlete()
            {
                Id = 1,
                FirstName = "test1 edited",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            this.mockMapper
                .Setup(m => m.Map(athleteEditModel, It.IsAny<Athlete>()))
                .Callback<EditAthleteServiceModel, Athlete>((src, dest) =>
                {
                    dest.FirstName = src.FirstName;
                    dest.LastName = src.LastName;
                    dest.PhoneNumber = src.PhoneNumber;
                    dest.UserId = src.UserId;
                });
            await this.service.EditAsync(athleteEditModel);

            var result = await this.data.Athletes.FirstAsync(a => a.Id == 1);
            result.FirstName.Should().Be("test1 edited");
            result.UserId.Should().Be("test-user-id1");
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowInvalidOperationException_IfIdPassedIsNotValid()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.DeleteAsync(12231));
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDelete_AthleteAndAthleteMembership()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            await this.service.DeleteAsync(1);

            var athleteDeleted = await this.data.Athletes.FirstAsync(a => a.Id == 1);
            athleteDeleted.IsDeleted.Should().BeTrue();
            athleteDeleted.MembershipId.Should().Be(1);

            var membershipDeleted = await this.data.Memberships.FirstAsync(a => a.Id == 1);
            membershipDeleted.IsDeleted.Should().BeTrue();
            membershipDeleted.AthleteId.Should().Be(1);
        }

        [Fact]
        public async Task AthleteHasApprovedMembershipByAthleteIdAsync_ShouldReturnTrue_IfAthleteHasApprovedMembership()
        {
            this.fixture.ResetDb();
            var membership = new Membership
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 12,
                WorkoutsLeft = 12,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = false,
            };

            var athlete = new Athlete
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteHasApprovedMembershipByAthleteIdAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AthleteHasApprovedMembershipByAthleteIdAsync_ShouldReturnFalse_IfAthleteHasMembershipIsPending()
        {
            this.fixture.ResetDb();
            var membership = new Membership
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 12,
                WorkoutsLeft = 12,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteHasApprovedMembershipByAthleteIdAsync(1);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AthleteHasMembershipByAthleteIdAsync_ShouldReturnTrue_IfAthleteHasMembership()
        {
            this.fixture.ResetDb();
            var membership = new Membership
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 12,
                WorkoutsLeft = 12,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = false,
            };

            var athlete = new Athlete
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteHasMembershipByAthleteIdAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AthleteHasMembershipByAthleteIdAsync_ShouldReturnFalse_IfAthleteHasNotMembership()
        {
            this.fixture.ResetDb();

            var athlete = new Athlete
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = null,
            };

            this.data.Add(athlete);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteHasMembershipByAthleteIdAsync(1);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UserIsAthleteByUserIdAsync_ShouldReturnFalse_IfAthleteWithSuchUserIdDoNotExist() 
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var result = await this.service.UserIsAthleteByUserIdAsync("faked-Id");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UserIsAthleteByUserIdAsync_ShouldReturnTrue_IfAthleteWithSuchUserIdExists()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var result = await this.service.UserIsAthleteByUserIdAsync("test-user-id1");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AthleteWithSameNumberExistsAsync_ShouldReturnFalse_IfSuchDoNotExist()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var result = await this.service.AthleteWithSameNumberExistsAsync("fake-phone");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AthleteWithSameNumberExistsAsync_ShouldReturnTrue_IfThereIsSuch()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var result = await this.service.AthleteWithSameNumberExistsAsync("0000000001");
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetIdByUserIdAsync_ShouldThrowInvalidOperationException_IfAthleteWithSuchIdDoNotExist()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.GetIdByUserIdAsync("fake-userid"));
        }

        [Fact]
        public async Task GetIdByUserIdAsync_ShouldReturnTheCorrectId_IfAthleteExists()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var result = await this.service.GetIdByUserIdAsync("test-user-id1");
            result.Should().Be(1);
        }

        [Fact]
        public async Task AthleteAlreadyJoinedByIdAsync_ShouldReturnFalse_IfAthleteHasNotJoined() 
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 10,
                CurrentParticipants = 0,
            };
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            var mapModel = new AthleteWorkout()
            {
                AthleteId = 2,
                WorkoutId = 1
            };

            this.data.Add(mapModel);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteAlreadyJoinedByIdAsync(1, 1);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AthleteAlreadyJoinedByIdAsync_ShouldReturnTrue_IfAthleteHasJoined()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 10,
                CurrentParticipants = 0,
            };
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            var mapModel = new AthleteWorkout()
            {
                AthleteId = 1,
                WorkoutId = 1
            };

            this.data.Add(mapModel);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteAlreadyJoinedByIdAsync(1, 1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AthleteMembershipIsExpiredByIdAsync_ShouldThrowInvalidOperationException_IfMembershipNotFound()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var athlete3 = new Athlete()
            {
                Id = 3,
                FirstName = "athl3",
                LastName = "athl3",
                PhoneNumber = "0000000003",
                Email = null,
                UserId = "test-user-id3",
                MembershipId = null
            };

            this.data.Add(athlete3);
            await this.data.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.AthleteMembershipIsExpiredByIdAsync(3));
        }

        [Fact]
        public async Task AthleteMembershipIsExpiredByIdAsync_ShouldRetrunTrue_IfMembershipIsExpired()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var membership3 = new Membership()
            {
                Id = 3,
                IsMonthly = true,
                StartDate = DateTime.UtcNow.AddMonths(-2),
                EndDate = DateTime.UtcNow.AddMonths(-1),
                TotalWorkoutsCount = null,
                WorkoutsLeft = null,
                Price = 200,
                IsPending = false,
            };

            var athlete3 = new Athlete()
            {
                Id = 3,
                FirstName = "athl3",
                LastName = "athl3",
                PhoneNumber = "0000000003",
                Email = null,
                UserId = "test-user-id3",
                MembershipId = 3
            };

            membership3.AthleteId = 3;
            this.data.Add(athlete3);
            this.data.Add(membership3);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteMembershipIsExpiredByIdAsync(3);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AthleteMembershipIsExpiredByIdAsync_ShouldRetrunFalse_IfMembershipIsNotExpired()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var membership3 = new Membership()
            {
                Id = 3,
                IsMonthly = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                TotalWorkoutsCount = null,
                WorkoutsLeft = null,
                Price = 200,
                IsPending = false,
            };

            var athlete3 = new Athlete()
            {
                Id = 3,
                FirstName = "athl3",
                LastName = "athl3",
                PhoneNumber = "0000000003",
                Email = null,
                UserId = "test-user-id3",
                MembershipId = 3
            };

            membership3.AthleteId = 3;
            this.data.Add(athlete3);
            this.data.Add(membership3);
            await this.data.SaveChangesAsync();

            var result = await this.service.AthleteMembershipIsExpiredByIdAsync(3);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetDetailsModelByIdAsync_ShouldReturnNull_IfAthleteDoesNotExist()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 10,
                CurrentParticipants = 0,
            };
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            var mapModel = new AthleteWorkout()
            {
                AthleteId = 1,
                WorkoutId = 1
            };

            this.data.Add(mapModel);
            await this.data.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();
            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);

            var result = await this.service.GetDetailsModelByIdAsync(12213);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetDetailsModelByIdAsync_ShouldReturn_TheCorrectModel()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 10,
                CurrentParticipants = 0,
            };
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            var mapModel = new AthleteWorkout()
            {
                AthleteId = 1,
                WorkoutId = 1
            };

            this.data.Add(mapModel);
            await this.data.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();
            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);

            var result = await this.service.GetDetailsModelByIdAsync(1);
            result.Should().NotBeNull();
            result.FirstName.Should().Be("athl1");
            result.Membership.Should().NotBeNull();
            result.MembershipId.Should().Be(1);
            result.Workouts.First().Should().NotBeNull();
            result.Workouts.First().Id.Should().Be(1);
        }

        [Fact]
        public async Task GetEditServiceModelByIdAsync_ShouldReturnNull_IfModelDoesNotExist()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();
            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);

            var result = await this.service.GetEditServiceModelByIdAsync(24121);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetEditServiceModelByIdAsync_ShouldReturn_TheCorrectModel()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();
            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);

            var result = await this.service.GetEditServiceModelByIdAsync(1);
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.FirstName.Should().Be("athl1");
        }

        [Fact]
        public async Task GetPageModelsAsync_ShouldReturnPaginatedAthletes_WhenSearchTermIsProvided()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();
            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);

            var searchTerm = "athl1";
            var pageIndex = 1;
            var pageSize = 2; 
            var result = await this.service.GetPageModelsAsync(searchTerm, pageIndex, pageSize);

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(1);
            result.Athletes.Should().HaveCount(1);
            result.PageIndex.Should().Be(1);
            result.TotalPages.Should().Be(1); 
            result.Athletes.Should().Contain(a => a.FirstName == "athl1" && a.LastName == "athl1");
        }

        [Fact]
        public async Task GetPageModelsAsync_ShouldReturnEmptyList_WhenNoAthletesMatchSearchTerm()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();

            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);
            var searchTerm = "some-non-existing-name";
            var pageIndex = 1;
            var pageSize = 1;
            var result = await this.service.GetPageModelsAsync(searchTerm, pageIndex, pageSize);

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(0); 
            result.Athletes.Should().BeEmpty();
            result.PageIndex.Should().Be(1);
            result.TotalPages.Should().Be(0);
        }

        [Fact]
        public async Task GetPageModelsAsync_ShouldReturnPaginatedAthletes_WhenSearchTermIsNotProvided()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AthleteProfile()));
            var realMapper = mapperConfig.CreateMapper();

            this.service = new AthleteService(this.data, this.mockMembershipService.Object, realMapper);
            string searchTerm = null!;
            var pageIndex = 1;
            var pageSize = 2;
            var result = await this.service.GetPageModelsAsync(searchTerm, pageIndex, pageSize);

            result.Should().NotBeNull();
            result.TotalCount.Should().Be(2);
            result.Athletes.Should().HaveCount(2);
            result.PageIndex.Should().Be(1);
            result.TotalPages.Should().Be(1);
            result.Athletes.Should().Contain(a => a.FirstName == "athl1" && a.LastName == "athl1");
            result.Athletes.Should().Contain(a => a.FirstName == "athl2" && a.LastName == "athl2");
        }

        [Fact]
        public async Task JoinAsync_ShouldThrowInvalidOperationException_IfWorkoutIsNotFound() 
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 10,
                CurrentParticipants = 0,
            };
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.JoinAsync(1, 2414142));
            exception.Message.Should().Be("Workout not found!");
        }

        [Fact]
        public async Task JoinAsync_ShouldThrowWorkoutClosedException_IfWorkoutCurrentParticipansAreEqualToTheMax()
        {
            this.fixture.ResetDb();
            await this.SeedDbAsync();

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 12,
            };
            this.data.Add(workout);
            await this.data.SaveChangesAsync();

            await Assert.ThrowsAsync<WorkoutClosedException>(() => this.service.JoinAsync(1, 1));
        }

        [Fact]
        public async Task JoinAsync_ShouldThrowInvalidOperationException_IfAthleteMembershipIsNull()
        {
            this.fixture.ResetDb();

            var athlete = new Athlete
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = null,
            };

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            await this.data.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.JoinAsync(1, 1));
            exception.Message.Should().Be("Athlete has not active membership!");
        }

        [Fact]
        public async Task JoinAsync_ShouldThrowMembershipExpiredException_IfAthleteMembershipIsExpired()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = true,
                StartDate = DateTime.UtcNow.AddMonths(-1).AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(-1),
                TotalWorkoutsCount = null,
                WorkoutsLeft = null,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            await Assert.ThrowsAsync<MembershipExpiredException>(() => this.service.JoinAsync(1, 1));
        }

        [Fact]
        public async Task JoinAsync_ShouldWorkProperly()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 10,
                WorkoutsLeft = 10,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            await this.service.JoinAsync(1, 1);

            var mapEntity = await this.data.AthletesWorkouts.FirstAsync();
            mapEntity.AthleteId.Should().Be(1);
            mapEntity.WorkoutId.Should().Be(1);

            var workoutResult = await this.data.Workouts.FirstAsync();
            workoutResult.CurrentParticipants.Should().Be(1);

            var membershipResult = await this.data.Memberships.FirstAsync();
            membershipResult.WorkoutsLeft.Should().Be(9);
        }

        [Fact]
        public async Task JoinAsync_ShouldSoftDeleteAthleteMembership_IfWorkoutsLeftAre1()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 10,
                WorkoutsLeft = 1,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            this.mockMembershipService
              .Setup(m => m.DeleteByIdAsync(membership.Id))
              .Callback(() =>
              {
                  membership.IsDeleted = true;
                  athlete.MembershipId = null;
              })
              .Returns(Task.CompletedTask);

            await this.service.JoinAsync(1, 1);

            var membershipResult = await this.data.Memberships.FirstAsync();
            membershipResult.IsDeleted.Should().BeTrue();

            var athleteResult = await this.data.Athletes.FirstAsync();
            athleteResult.MembershipId.Should().BeNull();
        }

        [Fact]
        public async Task LeaveAsync_ShouldThrowInvalidOperationException_IfAthleteWithSuchIdDoesNotExist()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 10,
                WorkoutsLeft = 10,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            await this.service.JoinAsync(1, 1);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.LeaveAsync(2321, 1));
            exception.Message.Should().Be("Athlete not found!");
        }

        [Fact]
        public async Task LeaveAsync_ShouldThrowInvalidOperationException_IfWorkoutWithSuchIdDoesNotExist()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 10,
                WorkoutsLeft = 10,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            await this.service.JoinAsync(1, 1);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.LeaveAsync(1, 21412211));
            exception.Message.Should().Be("Workout not found!");
        }

        [Fact]
        public async Task LeaveAsync_ShouldThrowInvalidOperationException_IfMapEntityDoesNotExist()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 10,
                WorkoutsLeft = 10,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete1 = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            var athlete2 = new Athlete()
            {
                Id = 2,
                FirstName = "athl1",
                LastName = "athl2",
                PhoneNumber = "0000000002",
                Email = null,
                UserId = "test-user-id2",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.AddRange(athlete1, athlete2);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            await this.service.JoinAsync(1, 1);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.LeaveAsync(2, 1));
            exception.Message.Should().Be("Map entity not found!");
        }

        [Fact]
        public async Task LeaveAsync_ShouldWorkProperly()
        {
            this.fixture.ResetDb();

            var membership = new Membership()
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 10,
                WorkoutsLeft = 10,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = 1,
            };

            membership.AthleteId = 1;

            var workout = new Workout()
            {
                Id = 1,
                Title = "workout1",
                Date = DateTime.UtcNow,
                Time = TimeSpan.FromHours(1),
                BriefDescription = null,
                FullDescription = null,
                ImageUrl = null,
                CategoryName = new Category() { Name = "CrossFit" },
                IsForBeginners = true,
                MaxParticipants = 12,
                CurrentParticipants = 0,
            };

            this.data.Add(workout);
            this.data.Add(athlete);
            this.data.Add(membership);
            await this.data.SaveChangesAsync();

            await this.service.JoinAsync(1, 1);
            await this.service.LeaveAsync(1, 1);

            var workoutResult = await this.data.Workouts.FirstAsync();
            workoutResult.CurrentParticipants.Should().Be(0);

            var membershipResult = await this.data.Memberships.FirstAsync();
            membershipResult.WorkoutsLeft.Should().Be(10);

            var mapEntity = await this.data.AthletesWorkouts.FirstOrDefaultAsync();
            mapEntity.Should().BeNull();
        }

        private async Task SeedDbAsync()
        {
            var membership1 = new Membership
            {
                Id = 1,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 12,
                WorkoutsLeft = 12,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = true,
            };

            var membership2 = new Membership
            {
                Id = 2,
                IsMonthly = false,
                StartDate = DateTime.UtcNow,
                EndDate = null,
                TotalWorkoutsCount = 12,
                WorkoutsLeft = 12,
                Price = 96,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                IsPending = false,
            };

            var athlete1 = new Athlete
            {
                Id = 1,
                FirstName = "athl1",
                LastName = "athl1",
                PhoneNumber = "0000000001",
                Email = null,
                UserId = "test-user-id1",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = membership1.Id,
            };

            var athlete2 = new Athlete
            {
                Id = 2,
                FirstName = "athl2",
                LastName = "athl2",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id2",
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                MembershipId = membership2.Id,
            };

            membership1.AthleteId = 1;
            membership2.AthleteId = 2;

            await this.data.Athletes.AddRangeAsync(athlete1, athlete2);
            await this.data.Memberships.AddRangeAsync(membership1, membership2);
            await this.data.SaveChangesAsync();
        }
    }
}
