namespace SoldierTrack.Tests.Membership
{
    using AutoMapper;
    using Common;
    using FluentAssertions;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Services.Athlete;
    using Services.Membership;
    using Services.Membership.MapperProfile;
    using Services.Membership.Models;
    using SoldierTrack.Common.Settings;
    using Xunit;

    public class MembershipServiceTests : ServiceTestBase
    {
        private readonly Mock<IAthleteService> athleteServiceMock;
        private readonly MembershipService service;

        public MembershipServiceTests()
        {
            this.athleteServiceMock = new Mock<IAthleteService>();

            var mapper = new MapperConfiguration(
                    config => config.AddProfile<MembershipProfile>(),
                    NullLoggerFactory.Instance)
                .CreateMapper();

            var adminOptions = Options.Create(new AdminSettings
            {
                Email = "admin@example.com",
                Password = "Admin123!"
            });

            this.service = new MembershipService(
                this.data,
                new Lazy<IAthleteService>(() => this.athleteServiceMock.Object),
                adminOptions,
                mapper);
        }

        [Fact]
        public async Task MembershipExistsByAthleteId_WhenMembershipExists_ReturnsTrue()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(athlete.Id);

            var result = await this.service.MembershipExistsByAthleteId(athlete.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task MembershipExistsByAthleteId_WhenNoMembership_ReturnsFalse()
        {
            var result = await this.service.MembershipExistsByAthleteId("no-athlete");

            result.Should().BeFalse();
        }

        [Fact]
        public async Task MembershipIsApprovedByAthleteId_WhenPending_ReturnsFalse()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(athlete.Id, isPending: true);

            var result = await this.service.MembershipIsApprovedByAthleteId(athlete.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task MembershipIsApprovedByAthleteId_WhenApproved_ReturnsTrue()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(athlete.Id, isPending: false);

            var result = await this.service.MembershipIsApprovedByAthleteId(athlete.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task MembershipIsExpiredByAthleteId_WhenMonthlyAndPastEndDate_ReturnsTrue()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: true,
                isPending: false,
                endDate: DateTime.UtcNow.AddDays(-1));

            var result = await this.service.MembershipIsExpiredByAthleteId(athlete.Id);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task MembershipIsExpiredByAthleteId_WhenMonthlyAndFutureEndDate_ReturnsFalse()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: true,
                isPending: false,
                endDate: DateTime.UtcNow.AddDays(30));

            var result = await this.service.MembershipIsExpiredByAthleteId(athlete.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetPendingCount_ReturnsTotalPendingMemberships()
        {
            var athleteA = this.SeedAthlete("a-1", email: "a1@example.com");
            var athleteB = this.SeedAthlete("a-2", email: "a2@example.com");
            var athleteC = this.SeedAthlete("a-3", email: "a3@example.com");

            this.SeedMembership(athleteA.Id, isPending: true);
            this.SeedMembership(athleteB.Id, isPending: true);
            this.SeedMembership(athleteC.Id, isPending: false);

            var count = await this.service.GetPendingCount();

            count.Should().Be(2);
        }

        [Fact]
        public async Task UpdateMembershipOnJoinByAthleteId_WhenNonMonthlyAndApproved_DecrementsWorkoutsLeft()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: false,
                isPending: false,
                workoutsLeft: 8);

            await this.service.UpdateMembershipOnJoinByAthleteId(athlete.Id);

            var membership = this.data.Memberships.Single();
            membership.WorkoutsLeft.Should().Be(7);
        }

        [Fact]
        public async Task UpdateMembershipOnJoinByAthleteId_WhenMembershipIsPending_DoesNotDecrement()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: false,
                isPending: true,
                workoutsLeft: 8);

            await this.service.UpdateMembershipOnJoinByAthleteId(athlete.Id);

            this.data
                .Memberships
                .Single()
                .WorkoutsLeft
                .Should()
                .Be(8);
        }

        [Fact]
        public async Task UpdateMembershipOnLeaveByAthleteId_WhenNonMonthlyAndApproved_IncrementsWorkoutsLeft()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: false,
                isPending: false,
                workoutsLeft: 6);

            await this.service.UpdateMembershipOnLeaveByAthleteId(athlete.Id);

            this
                .data
                .Memberships
                .Single()
                .WorkoutsLeft
                .Should()
                .Be(7);
        }

        [Fact]
        public async Task UpdateMembershipOnWorkoutDeletion_WhenNonMonthly_IncrementsWorkoutsLeft()
        {
            var athlete = this.SeedAthlete();
            var membership = this.SeedMembership(
                athlete.Id,
                isMonthly: false,
                isPending: false,
                workoutsLeft: 4,
                totalWorkoutsCount: 10);

            await this.service.UpdateMembershipOnWorkoutDeletion(membership.Id);

            this
                .data
                .Memberships
                .Single()
                .WorkoutsLeft
                .Should()
                .Be(5);
        }

        [Fact]
        public async Task UpdateMembershipOnWorkoutDeletion_WhenNonMonthlyAndAtMax_DoesNotExceedTotal()
        {
            var athlete = this.SeedAthlete();
            var membership = this.SeedMembership(
                athlete.Id,
                isMonthly: false,
                isPending: false,
                workoutsLeft: 10,
                totalWorkoutsCount: 10);

            await this.service.UpdateMembershipOnWorkoutDeletion(membership.Id);

            this
                .data
                .Memberships
                .Single()
                .WorkoutsLeft
                .Should()
                .Be(10);
        }

        [Fact]
        public async Task UpdateMembershipOnWorkoutDeletion_WhenMonthly_DoesNotChangeWorkoutsLeft()
        {
            var athlete = this.SeedAthlete();
            var membership = this.SeedMembership(
                athlete.Id,
                isMonthly: true,
                isPending: false,
                workoutsLeft: 5);

            await this.service.UpdateMembershipOnWorkoutDeletion(membership.Id);

            this
                .data
                .Memberships
                .Single()
                .WorkoutsLeft
                .Should()
                .Be(5);
        }

        [Fact]
        public async Task DeleteIfExpired_WhenMonthlyMembershipIsExpired_SoftDeletesMembership()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: true,
                isPending: false,
                endDate: DateTime.UtcNow.AddDays(-1));

            await this.service.DeleteIfExpired(athlete.Id);

            this
                .data
                .Memberships
                .Single()
                .IsDeleted
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteIfExpired_WhenMonthlyMembershipIsNotExpired_DoesNotDelete()
        {
            var athlete = this.SeedAthlete();
            this.SeedMembership(
                athlete.Id,
                isMonthly: true,
                isPending: false,
                endDate: DateTime.UtcNow.AddDays(30));

            await this.service.DeleteIfExpired(athlete.Id);

            this
                .data
                .Memberships
                .Single()
                .IsDeleted
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task Approve_SetsMembershipAsNotPendingAndCallsSendMail()
        {
            var athlete = this.SeedAthlete();
            var membership = this.SeedMembership(
                athlete.Id,
                isPending: true);

            this.athleteServiceMock
                .Setup(s => s.SendMailForApproveMembership(athlete.Id))
                .Returns(Task.CompletedTask);

            await this.service.Approve(membership.Id);

            var updated = this.data.Memberships.Single();
            updated.IsPending.Should().BeFalse();

            this.athleteServiceMock.Verify(
                s => s.SendMailForApproveMembership(athlete.Id),
                Times.Once);
        }

        [Fact]
        public async Task Request_WithValidAthlete_CreatesMembershipWithPendingFlag()
        {
            var athlete = this.SeedAthlete();

            var model = new MembershipServiceModel
            {
                IsMonthly = false,
                IsPending = true,
                StartDate = DateTime.UtcNow,
                TotalWorkoutsCount = 12,
                WorkoutsLeft = 12,
                Price = 60,
                AthleteId = athlete.Id
            };

            await this.service.Request(model);

            var saved = this.data.Memberships.Single();
            saved.IsPending.Should().BeTrue();
            saved.AthleteId.Should().Be(athlete.Id);
        }
    }
}
