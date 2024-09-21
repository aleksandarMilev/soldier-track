namespace SoldierTrack.Tests.Services
{
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using SoldierTrack.Data;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;
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

            public void Dispose() => this.Data.Dispose();
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
            ClearDatabase(); 

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await SeedDatabase(athlete1, athlete2, membership1, membership2);
            var result = await service.GetAllPendingAsync();

            result.Should().HaveCount(1).And.ContainEquivalentOf(new MembershipPendingServiceModel()
                {
                    Id = membership1.Id,
                    StartDate = membership1.StartDate,
                    TotalWorkoutsCount = membership1.TotalWorkoutsCount,
                    IsMonthly = membership1.IsMonthly,
                    AthleteId = membership1.Athlete.Id,
                    EndDate = membership1.EndDate,
                    Price = membership1.Price,
                    AthleteName = $"{athlete1.FirstName} {athlete1.LastName}"
                });
        }

        [Fact]
        public async Task GetAllPendingCountAsync_ShouldReturnTheCorrectCount()
        {
            this.ClearDatabase();

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await this.SeedDatabase(athlete1, athlete2, membership1, membership2);
            var result = await this.service.GetPendingCountAsync();

            result.Should().Be(1);
        }

        [Fact]
        public async Task GetEditModelByIdAsync_ShouldReturnTheCorrectModel()
        {
            this.ClearDatabase();

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await this.SeedDatabase(athlete1, athlete2, membership1, membership2);
            var actualResult = await this.service.GetEditModelByIdAsync(membership1.Id);
            var expectedResult = new EditMembershipServiceModel()
            {
                Id = actualResult!.Id,
                StartDate = actualResult.StartDate,
                TotalWorkoutsCount = actualResult.TotalWorkoutsCount,
                IsMonthly = actualResult.IsMonthly,
                AthleteId = actualResult.AthleteId,
                EndDate = actualResult.EndDate,
                Price = actualResult.Price,
                IsPending = actualResult.IsPending,
                WorkoutsLeft = actualResult.WorkoutsLeft
            };

            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetEditModelByIdAsync_ShouldReturnNullIfTheIdPassedAsArgumentIsNotValid()
        {
            this.ClearDatabase();

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await this.SeedDatabase(athlete1, athlete2, membership1, membership2);
            var actualResult = await this.service.GetEditModelByIdAsync(21412421);

            actualResult.Should().Be(null);
        }

        [Fact]
        public async Task RequestAsync_ShouldThrowInvalidOperationExceptionIfAthleteEntityIsNotFound()
        {
            this.ClearDatabase();

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = null,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            var requestModel = new CreateMembershipServiceModel()
            {
                StartDate = DateTime.UtcNow,
                IsMonthly = false,
                TotalWorkoutsCount = 15,
                AthleteId = 124214,
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.RequestAsync(requestModel));
        }

        [Fact]
        public async Task RequestAsync_ShouldWorkProperlyWithValidModel()
        {
            this.ClearDatabase(); 

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = null,  
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };

            this.data.Athletes.Add(athlete);
            await this.data.SaveChangesAsync();

            var requestModel = new CreateMembershipServiceModel()
            {
                StartDate = DateTime.UtcNow,
                IsMonthly = true,
                TotalWorkoutsCount = null,
                AthleteId = athlete.Id, 
            };
            await this.service.RequestAsync(requestModel);

            var membership = await this.data
                .Memberships
                .Include(m => m.Athlete)
                .FirstOrDefaultAsync(m => m.Athlete.Id == athlete.Id);

            membership.Should().NotBeNull();
            membership!.StartDate.Should().Be(requestModel.StartDate);
            membership.IsMonthly.Should().BeTrue();
            membership.TotalWorkoutsCount.Should().Be(null);
            membership.Athlete.Id.Should().Be(athlete.Id);
            membership.Price.Should().Be(requestModel.Price);
            membership.IsPending.Should().Be(true);

            var updatedAthlete = await this.data
                .Athletes
                .FirstOrDefaultAsync(a => a.Id == athlete.Id);

            updatedAthlete!.MembershipId.Should().Be(membership.Id);
        }

        [Fact]
        public async Task ApproveAsync_ShouldThrowInvalidOperationExceptionIfIdPassedIsNotValid()
        {
            this.ClearDatabase();

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = null,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            this.data.Athletes.Add(athlete);
            await this.data.SaveChangesAsync();

            var requestModel = new CreateMembershipServiceModel()
            {
                StartDate = DateTime.UtcNow,
                IsMonthly = true,
                TotalWorkoutsCount = null,
                AthleteId = athlete.Id,
            };
            await this.service.RequestAsync(requestModel);

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.ApproveAsync(1232113));
        }

        [Fact]
        public async Task ApproveAsync_ShouldSetTheMembershipIsPendingToFalse()
        {
            this.ClearDatabase();

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = null,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            this.data.Athletes.Add(athlete);
            await this.data.SaveChangesAsync();

            var requestModel = new CreateMembershipServiceModel()
            {
                StartDate = DateTime.UtcNow,
                IsMonthly = true,
                TotalWorkoutsCount = null,
                AthleteId = athlete.Id,
            };
            await this.service.RequestAsync(requestModel);

            var membership = await this.data.Memberships.SingleAsync();
            await this.service.ApproveAsync(membership.Id);

            Assert.False(membership.IsPending);
        }

        [Fact]
        public async Task RejectAsync_ShouldThrowInvalidOperationExceptionIfIdPassedIsNotValid()
        {
            this.ClearDatabase();

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = null,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            this.data.Athletes.Add(athlete);
            await this.data.SaveChangesAsync();

            var requestModel = new CreateMembershipServiceModel()
            {
                StartDate = DateTime.UtcNow,
                IsMonthly = true,
                TotalWorkoutsCount = null,
                AthleteId = athlete.Id,
            };
            await this.service.RequestAsync(requestModel);

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.RejectAsync(1232113));
        }

        [Fact]
        public async Task RejectAsync_ShouldSoftDeleteTheMembership()
        {
            this.ClearDatabase();

            var athlete = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = null,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            this.data.Athletes.Add(athlete);
            await this.data.SaveChangesAsync();

            var requestModel = new CreateMembershipServiceModel()
            {
                StartDate = DateTime.UtcNow,
                IsMonthly = true,
                TotalWorkoutsCount = null,
                AthleteId = athlete.Id,
            };
            await this.service.RequestAsync(requestModel);

            var membershipId = await this.data.Memberships.Select(m => m.Id).SingleAsync();
            await this.service.RejectAsync(membershipId);

            var membership = await this.data.Memberships.SingleAsync();

            Assert.True(membership.IsDeleted);
            Assert.Null(this.data.Athletes.Single().MembershipId);
        }

        [Fact]
        public async Task EditAsync_ShouldThrowInvalidOperationExceptionIfIdPassedIsNotValid() 
        {
            this.ClearDatabase();

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await this.SeedDatabase(athlete1, athlete2, membership1, membership2);
            var editModel = new EditMembershipServiceModel()
            {
                Id = 32132414,
                StartDate = membership1.StartDate,
                TotalWorkoutsCount = membership1.TotalWorkoutsCount,
                IsMonthly = membership1.IsMonthly,
                AthleteId = membership1.Athlete.Id,
                IsPending = membership1.IsPending,
                EndDate = membership1.EndDate,
                WorkoutsLeft = membership1.WorkoutsLeft,
                Price = membership1.Price,
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.EditAsync(editModel));
        }

        [Fact]
        public async Task EditAsync_ShouldEditTheMembership()
        {
            this.ClearDatabase();

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await this.SeedDatabase(athlete1, athlete2, membership1, membership2);
            var editModel = new EditMembershipServiceModel()
            {
                Id = membership1.Id,
                StartDate = membership1.StartDate,
                TotalWorkoutsCount = membership1.TotalWorkoutsCount,
                IsMonthly = membership1.IsMonthly,
                AthleteId = membership1.Athlete.Id,
                IsPending = membership1.IsPending,
                EndDate = membership1.EndDate,
                WorkoutsLeft = membership1.WorkoutsLeft,
                Price = membership1.Price,
            };
            await this.service.EditAsync(editModel);

            var membershipEntity = await this.data.Memberships.FirstAsync(m => m.Id == membership1.Id);

            membership1.Should().BeEquivalentTo(editModel, opt => opt.Excluding(m => m.AthleteId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowInvalidOperationExceptionIfIdPassedIsNotValid()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteTheMembership()
        {
            this.ClearDatabase();

            CreateAthletesAndMembershipsEntityModels(
                out Athlete athlete1,
                out Athlete athlete2,
                out Membership membership1,
                out Membership membership2);

            await this.SeedDatabase(athlete1, athlete2, membership1, membership2);
            await this.service.DeleteAsync(membership1.Id);

            var membershipEntity = await this.data.Memberships.FirstAsync(m => m.Id == membership1.Id);
            var athleteEntity = await this.data.Athletes.FirstAsync(a => a.Id == athlete1.Id);

            Assert.True(membershipEntity.IsDeleted);
            Assert.Null(athlete1.MembershipId);
        }

        private async Task SeedDatabase(
            Athlete athlete1,
            Athlete athlete2,
            Membership membership1,
            Membership membership2)
        {
            this.data.Memberships.AddRange(membership1, membership2);
            this.data.Athletes.AddRange(athlete1, athlete2);
            await this.data.SaveChangesAsync();
        }

        private void ClearDatabase()
        {
            this.data.Memberships.RemoveRange(this.data.Memberships);
            this.data.Athletes.RemoveRange(this.data.Athletes);
            this.data.SaveChanges();
        }

        private static void CreateAthletesAndMembershipsEntityModels(
            out Athlete athlete1,
            out Athlete athlete2,
            out Membership membership1,
            out Membership membership2)
        {
            athlete1 = new Athlete()
            {
                Id = 1,
                FirstName = "test1",
                LastName = "test1",
                PhoneNumber = "0000000000",
                Email = null,
                UserId = "test-user-id1",
                MembershipId = 1,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            athlete2 = new Athlete()
            {
                Id = 2,
                FirstName = "test2",
                LastName = "test2",
                PhoneNumber = "1111111111",
                Email = null,
                UserId = "test-user-id2",
                MembershipId = 2,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = DateTime.UtcNow
            };
            membership1 = new Membership()
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
                Athlete = athlete1,
                IsPending = true
            };
            membership2 = new Membership()
            {
                Id = 2,
                IsMonthly = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                TotalWorkoutsCount = null,
                WorkoutsLeft = null,
                Price = 200,
                CreatedOn = DateTime.UtcNow,
                DeletedOn = null,
                IsDeleted = false,
                ModifiedOn = null,
                Athlete = athlete2,
                IsPending = false
            };
        }
    }
}
