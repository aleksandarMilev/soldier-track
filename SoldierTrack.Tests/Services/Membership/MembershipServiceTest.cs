namespace SoldierTrack.Tests.Services.Membership
{
    using AutoMapper;
    using Data.Models;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Moq;
    using SoldierTrack.Common.Settings;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Models;
    using Xunit;

    public class MembershipServiceTest : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture fixture;
        private readonly Mock<IAthleteService> mockAthleteService;
        private readonly MembershipService membershipService;
        private readonly IMapper mapper;

        public MembershipServiceTest(TestDatabaseFixture fixture)
        {
            this.fixture = fixture;

            this.mockAthleteService = new Mock<IAthleteService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Membership, MembershipServiceModel>().ReverseMap();
                cfg.CreateMap<MembershipArchive, MembershipServiceModel>();
            });

            this.mapper = config.CreateMapper();

            var fakeAdminSettings = new AdminSettings()
            {
                Email = "admin@mail.com",
                Password = "admin1234"
            };

            var fakeAdminOptions = Options.Create(fakeAdminSettings);

            this.membershipService = new MembershipService(
                this.fixture.Data,
                new Lazy<IAthleteService>(() => this.mockAthleteService.Object),
                fakeAdminOptions,
                this.mapper
            );
        }

        private void ResetDatabase()
        {
            this.fixture.ResetDatabase();
            SeedMemberships();
        }

        private void SeedMemberships()
        {
            if (this.fixture.Data.Memberships.Any())
            {
                return;
            }

            var memberships = new Membership[]
            {
                new()
                {
                    Id = 1,
                    AthleteId = "1",
                    IsPending = true,
                    IsMonthly = true,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    TotalWorkoutsCount = 10,
                    WorkoutsLeft = 10,
                    Price = 50
                },
                new()
                {
                    Id = 2,
                    AthleteId = "2",
                    IsPending = false,
                    IsMonthly = true,
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(-1),
                    TotalWorkoutsCount = 15,
                    WorkoutsLeft = 5,
                    Price = 75
                }
            };

            this.fixture.Data.Memberships.AddRange(memberships);
            this.fixture.Data.SaveChanges();
        }

        [Fact]
        public async Task GetPendingCountAsync_ShouldReturnCorrectCount()
        {
            this.ResetDatabase();

            var pendingCount = await this.membershipService.GetPendingCountAsync();

            pendingCount.Should().Be(1);
        }

        [Fact]
        public async Task MembershipExistsByAthleteIdAsync_ShouldReturnTrue_WhenMembershipExists()
        {
            this.ResetDatabase();

            var exists = await this.membershipService.MembershipExistsByAthleteId("1");

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task MembershipIsExpiredByAthleteIdAsync_ShouldReturnTrue_WhenMembershipIsExpired()
        {
            this.ResetDatabase();

            var isExpired = await this.membershipService.MembershipIsExpiredByAthleteIdAsync("2");

            isExpired.Should().BeTrue();
        }

        [Fact]
        public async Task ApproveAsync_ShouldApproveMembership_WhenValidIdIsProvided()
        {
            this.ResetDatabase();

            await this.membershipService.Approve(1);

            var membership = await this.fixture.Data.Memberships.FindAsync(1);
            membership.Should().NotBeNull();
            membership?.IsPending.Should().BeFalse();
            membership?.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task ApproveAsync_ShouldThrowException_WhenInvalidIdIsProvided()
        {
            this.ResetDatabase();

            Func<Task> act = async () => await this.membershipService.Approve(999);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Membership not found!");
        }

        [Fact]
        public async Task RejectAsync_ShouldThrowException_WhenInvalidIdIsProvided()
        {
            this.ResetDatabase();

            Func<Task> act = async () => await this.membershipService.Reject(999);

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Membership not found!");
        }
    }
}
