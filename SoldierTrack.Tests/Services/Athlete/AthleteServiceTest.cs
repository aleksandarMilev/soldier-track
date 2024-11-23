namespace SoldierTrack.Tests.Services.Athlete
{
    using AutoMapper;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Data.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Services.Email;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Models;
    using Xunit;

    public class AthleteServiceTest : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture fixture;
        private readonly Mock<IMembershipService> mockMembershipService;
        private readonly Mock<IEmailService> mockEmailService;
        private readonly AthleteService athleteService;
        private readonly IMapper mapper;

        public AthleteServiceTest(TestDatabaseFixture fixture)
        {
            this.fixture = fixture;

            this.mockMembershipService = new Mock<IMembershipService>();
            this.mockEmailService = new Mock<IEmailService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Athlete, AthleteServiceModel>().ReverseMap();
                cfg.CreateMap<Athlete, AthleteDetailsServiceModel>();
                cfg.CreateMap<Membership, MembershipServiceModel>();
            });

            this.mapper = config.CreateMapper();
            this.athleteService = CreateAthleteService();
        }

        private AthleteService CreateAthleteService()
            => new(
                this.fixture.Data,
                new Lazy<IMembershipService>(() => this.mockMembershipService.Object),
                this.mockEmailService.Object,
                this.mapper);

        private void ResetDatabase()
        {
            this.fixture.ResetDatabase();
            SeedAthletes();
        }

        private void SeedAthletes()
        {
            var athletes = new Athlete[]
            {
                new()
                {
                    Id = "1",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Membership = new()
                    {
                        Id = 1,
                        IsPending = false,
                        IsMonthly = true,
                        StartDate = DateTime.UtcNow.AddMonths(-2),
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        WorkoutsLeft = 20,
                        AthleteId = "1"
                    }
                },
                new()
                {
                    Id = "2",
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@example.com",
                    Membership = new()
                    {
                        Id = 2,
                        IsPending = false,
                        IsMonthly = true,
                        StartDate = DateTime.UtcNow.AddMonths(-1),
                        EndDate = DateTime.UtcNow.AddMonths(2),
                        WorkoutsLeft = 15,
                        AthleteId = "2"
                    }
                }
            };

            this.fixture.Data.Athletes.AddRange(athletes);
            this.fixture.Data.SaveChanges();
        }

        [Fact]
        public async Task GetPageModelsAsync_ShouldReturnPagedAthletes_WhenAthletesExist()
        {
            this.ResetDatabase();

            var result = await this.athleteService.GetPageModelsAsync(null, 1, 2);

            result.Should().NotBeNull();
            result.Athletes.Should().HaveCount(2);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetNameByIdAsync_ShouldReturnAthleteName_WhenAthleteExists()
        {
            this.ResetDatabase();

            var result = await this.athleteService.GetNameByIdAsync("1");

            result.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetNameByIdAsync_ShouldReturnAdminRoleName_WhenAthleteDoesNotExist()
        {
            this.ResetDatabase();

            var result = await this.athleteService.GetNameByIdAsync("999");

            result.Should().Be("Administrator");
        }

        [Fact]
        public async Task GetModelByIdAsync_ShouldReturnAthleteModel_WhenAthleteExists()
        {
            this.ResetDatabase();

            var result = await this.athleteService.GetModelByIdAsync("1");

            result.Should().NotBeNull();
            result?.FirstName.Should().Be("John");
            result?.LastName.Should().Be("Doe");
        }

        [Fact]
        public async Task EditAsync_ShouldUpdateAthlete_WhenValidModelIsProvided()
        {
            this.ResetDatabase();

            var athlete = this.fixture.Data.Athletes.First();
            var model = new AthleteServiceModel()
            {
                Id = athlete.Id,
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated.email@example.com",
                PhoneNumber = "123000000"
            };

            await this.athleteService.EditAsync(model);

            var updatedAthlete = await this.fixture.Data.Athletes.FindAsync(athlete.Id);

            updatedAthlete.Should().NotBeNull();
            updatedAthlete?.FirstName.Should().Be("Updated");
            updatedAthlete?.Email.Should().Be("updated.email@example.com");
        }

        [Fact]
        public async Task EditAsync_ShouldThrowException_WhenAthleteNotFound()
        {
            this.ResetDatabase();

            var model = new AthleteServiceModel()
            {
                Id = "999",
                FirstName = "NonExistent",
                LastName = "User",
                Email = "non.existent@example.com",
                PhoneNumber = "000000000"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.athleteService.EditAsync(model));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAthlete_WhenValidIdIsProvided()
        {
            this.ResetDatabase();

            var athlete = this.fixture.Data.Athletes.First();

            await this.athleteService.DeleteAsync(athlete.Id);

            athlete.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenAthleteNotFound()
        {
            this.ResetDatabase();

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.athleteService.DeleteAsync("999"));
        }

        [Fact]
        public async Task JoinAsync_ShouldAddAthleteToWorkout_WhenValidIdsAreProvided()
        {
            this.ResetDatabase();

            var athleteId = "1";
            var workoutId = 1;

            await this.athleteService.JoinAsync(athleteId, workoutId);

            var athleteWorkout = await this
                .fixture
                .Data
                .AthletesWorkouts
                .FirstOrDefaultAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId);

            athleteWorkout.Should().NotBeNull();
        }

        [Fact]
        public async Task LeaveAsync_ShouldRemoveAthleteFromWorkout_WhenValidIdsAreProvided()
        {
            this.ResetDatabase();

            var athleteId = "1";
            var workoutId = 1;

            await this.athleteService.JoinAsync(athleteId, workoutId);

            await this.athleteService.LeaveAsync(athleteId, workoutId);

            var athleteWorkout = await this
                .fixture
                .Data
                .AthletesWorkouts
                .FirstOrDefaultAsync(aw => aw.AthleteId == athleteId && aw.WorkoutId == workoutId);

            athleteWorkout.Should().BeNull();
        }
    }
}