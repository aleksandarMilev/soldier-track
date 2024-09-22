namespace SoldierTrack.Tests.Services
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Membership;
    using Xunit;

    using static SoldierTrack.Tests.Services.MembershipServiceTests;

    public class MembershipServiceTests : IClassFixture<TestDatabaseFixture>
    {
        public class TestDatabaseFixture : IDisposable
        {
            public TestDatabaseFixture()
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestMembershipDatabase")
                    .Options;

                this.Data = new ApplicationDbContext(options);
            }

            public ApplicationDbContext Data { get; private set; }

            public void Dispose() => GC.SuppressFinalize(this);
        }

        private readonly ApplicationDbContext data;
        private readonly MembershipService service;

        public MembershipServiceTests(TestDatabaseFixture fixture)
        {
            this.data = fixture.Data;
            this.service = new MembershipService(this.data);
        }

        [Fact]
        public async Task GetAllPendingAsync_ShouldReturnPendingMembershipsOnly()
        {
            // Arrange
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

            await this.data.Athletes.AddAsync(athlete1);
            await this.data.Memberships.AddRangeAsync(membership1, membership2);
            await this.data.SaveChangesAsync();

            // Act
            var result = await this.service.GetAllPendingAsync();

            // Assert
            result.Should().HaveCount(1);
            var firstMembership = result.First();
            firstMembership.Id.Should().Be(membership1.Id);
            firstMembership.AthleteName.Should().Be("athl1 athl1");
            firstMembership.Price.Should().Be(membership1.Price);
            firstMembership.StartDate.Should().Be(membership1.StartDate);
        }
    }
}
