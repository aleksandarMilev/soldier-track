//namespace SoldierTrack.Tests.Services
//{
//    using FluentAssertions;
//    using Microsoft.EntityFrameworkCore;
//    using SoldierTrack.Data;
//    using SoldierTrack.Data.Models;
//    using SoldierTrack.Services.Email;
//    using SoldierTrack.Services.Email.Models;
//    using SoldierTrack.Services.Membership;
//    using SoldierTrack.Services.Membership.Models;
//    using SoldierTrack.Services.Membership.Models.Base;
//    using Xunit;

//    using static SoldierTrack.Tests.Services.MembershipServiceTests;

//    public class MembershipServiceTests : IClassFixture<TestDatabaseFixture>
//    {
//        public class TestDatabaseFixture : IDisposable
//        {
//            private readonly DbContextOptions<ApplicationDbContext> options;

//            public TestDatabaseFixture()
//            {
//                this.options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                    .UseInMemoryDatabase(databaseName: "TestMembershipDatabase")
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

//        private readonly ApplicationDbContext data;
//        private readonly MembershipService service;
//        private readonly TestDatabaseFixture fixture;

//        public MembershipServiceTests(TestDatabaseFixture fixture)
//        {
//            this.fixture = fixture;
//            this.data = fixture.Data;
//            this.service = new MembershipService(this.data, new EmailService(new SmtpSettings())); 
//        }

//        [Fact]
//        public async Task GetArchiveByAthleteIdAsync_ReturnsCorrectMemberships()
//        {
//            this.fixture.ResetDb();
//            await this.SeedDatabaseAsync();
//            await this.service.DeleteByIdAsync(1);
//            var athleteId = 1;
//            var pageIndex = 1;
//            var pageSize = 10;

//            var actualResult = await this.service.GetArchiveByAthleteIdAsync(athleteId, pageIndex, pageSize);
//            var membershipEntity = await this.data.Memberships.FirstAsync(m => m.Id == 1);
//            var expectedResult = new MembershipArchivePageServiceModel()
//            {
//                PageIndex = pageIndex,
//                PageSize = pageSize,
//                TotalPages = 1,
//                TotalCount = 1,
//                Memberships = new List<MembershipServiceModel>()
//                {
//                    new MembershipServiceModel()
//                    {
//                        StartDate = membershipEntity.StartDate,
//                        TotalWorkoutsCount = membershipEntity.TotalWorkoutsCount,
//                        IsMonthly = membershipEntity.IsMonthly,
//                        AthleteId = membershipEntity.AthleteId,
//                        IsPending = membershipEntity.IsPending,
//                        EndDate = membershipEntity.EndDate,
//                        Price = membershipEntity.Price,
//                        WorkoutsLeft = membershipEntity.WorkoutsLeft,
//                    }
//                }
//            };

//            actualResult.Should().NotBeNull();
//            actualResult.Should().BeEquivalentTo(expectedResult);
//        }

//        [Fact]
//        public async Task GetAllPendingAsync_ShouldReturnPendingMembershipsOnly()
//        {
//            this.fixture.ResetDb(); 
//            var membership = await SeedDatabaseAsync();

//            var result = await this.service.GetAllPendingAsync();
//            result.Should().HaveCount(1);

//            var membershipEntity = result.First();
//            membershipEntity.Id.Should().Be(membership.Id);
//            membershipEntity.AthleteName.Should().Be("athl1 athl1");
//            membershipEntity.Price.Should().Be(membership.Price);
//            membershipEntity.StartDate.Should().Be(membership.StartDate);
//            membershipEntity.EndDate.Should().Be(null);
//        }

//        [Fact]
//        public async Task GetPendingCountAsync_ShouldReturnTheCorrectCount()
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();
//            var result = await this.service.GetPendingCountAsync();
//            result.Should().Be(1);
//        }

//        [Fact]
//        public async Task GetEditModelByIdAsync_ShouldReturnNullIfEntityNotFound() 
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();
//            var result = await this.service.GetEditModelByIdAsync(214412);
//            result.Should().Be(null);
//        }

//        [Fact]
//        public async Task GetEditModelByIdAsync_ShouldReturnTheCorrectEntity()
//        {
//            this.fixture.ResetDb();
//            var membership = await this.SeedDatabaseAsync();

//            var actualResult = await this.service.GetEditModelByIdAsync(1);
//            var exceptedResult = new EditMembershipServiceModel()
//            {
//                Id = membership.Id,
//                StartDate = membership.StartDate,
//                TotalWorkoutsCount = membership.TotalWorkoutsCount,
//                IsMonthly = membership.IsMonthly,
//                AthleteId = membership.Athlete.Id,
//                EndDate = membership.EndDate,
//                Price = membership.Price,
//                IsPending = membership.IsPending,
//                WorkoutsLeft = membership.WorkoutsLeft,
//            };

//            actualResult.Should().BeEquivalentTo(exceptedResult);
//        }

//        [Fact]
//        public async Task RequestAsync_ShouldThrowInvalidOperationException()
//        {
//            this.fixture.ResetDb();

//            var athlete = new Athlete()
//            {
//                Id = 1,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = null
//            };

//            this.data.Add(athlete);
//            await this.data.SaveChangesAsync();

//            var requestModel = new CreateMembershipServiceModel()
//            {
//                StartDate = DateTime.UtcNow,
//                TotalWorkoutsCount = 12,
//                AthleteId = 121421421,
//                IsMonthly = false,
//            };
//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.RequestAsync(requestModel));
//        }

//        [Fact]
//        public async Task RequestAsync_ShouldCreateTheMembershipInTheDatabase()
//        {
//            this.fixture.ResetDb();

//            var athlete = new Athlete()
//            {
//                Id = 1,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = null
//            };

//            this.data.Add(athlete);
//            await this.data.SaveChangesAsync();

//            var requestModel = new CreateMembershipServiceModel()
//            {
//                StartDate = DateTime.UtcNow,
//                TotalWorkoutsCount = 12,
//                AthleteId = 1,
//                IsMonthly = false,
//            };
//            await this.service.RequestAsync(requestModel);

//            var result = await this.data.Memberships.FirstAsync();
//            result.Should().NotBe(null);
//            result.Id.Should().Be(1);
//            result.AthleteId.Should().Be(1);
//            result.IsPending.Should().Be(true);
//        }

//        [Fact]
//        public async Task ApproveAsync_ShouldThrowInvalidOperationExceptionIfEntityIsNotFound()
//        {
//            this.fixture.ResetDb();

//            var athlete = new Athlete()
//            {
//                Id = 1,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = null
//            };

//            this.data.Add(athlete);
//            await this.data.SaveChangesAsync();

//            var requestModel = new CreateMembershipServiceModel()
//            {
//                StartDate = DateTime.UtcNow,
//                TotalWorkoutsCount = 12,
//                AthleteId = 1,
//                IsMonthly = false,
//            };
//            await this.service.RequestAsync(requestModel);

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.ApproveAsync(2142121));
//        }

//        [Fact]
//        public async Task ApproveAsync_ShouldSetMembershipIsPendingToFalse()
//        {
//            this.fixture.ResetDb();

//            var athlete = new Athlete()
//            {
//                Id = 1,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = null
//            };

//            this.data.Add(athlete);
//            await this.data.SaveChangesAsync();

//            var requestModel = new CreateMembershipServiceModel()
//            {
//                StartDate = DateTime.UtcNow,
//                TotalWorkoutsCount = 12,
//                AthleteId = 1,
//                IsMonthly = false,
//            };
//            await this.service.RequestAsync(requestModel);
//            await this.service.ApproveAsync(1);

//            var result = await this.data.Memberships.FirstAsync();
//            result.IsPending.Should().Be(false);
//        }

//        [Fact]
//        public async Task EditAsync_ShouldThrowInvalidOperationExceptionIfModelIdIsInvalid() 
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();

//            var editModel = new EditMembershipServiceModel()
//            {
//                Id = 123212,
//                IsMonthly = false,
//                StartDate = DateTime.UtcNow,
//                EndDate = null,
//                TotalWorkoutsCount = 12,
//                WorkoutsLeft = 12,
//                Price = 96,
//                IsPending = true,
//            };

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.EditAsync(editModel));
//        }

//        [Fact]
//        public async Task EditAsync_ShouldEditTheEntity()
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();

//            var editModel = new EditMembershipServiceModel()
//            {
//                Id = 1,
//                IsMonthly = false,
//                StartDate = DateTime.UtcNow,
//                EndDate = null,
//                TotalWorkoutsCount = 15,
//                WorkoutsLeft = 15,
//                Price = 120,
//                IsPending = false,
//            };

//            await this.service.EditAsync(editModel);
//            var result = await this.data.Memberships.FirstAsync(m => m.Id == 1);
//            result.Should().NotBe(null);
//            result.TotalWorkoutsCount = 15;
//            result.WorkoutsLeft = 15;
//            result.Price = 120;
//            result.IsPending = true;
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldThrowInvalidOperationExceptionIfIdIsInvalid()
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();

//            await Assert.ThrowsAsync<InvalidOperationException>(() => this.service.DeleteByIdAsync(123121));
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldCreateMembershipArchiveEntityInTheDatabase()
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();

//            await this.service.DeleteByIdAsync(1);

//            var result = await this.data.MembershipArchives.FirstAsync();
//            result.Should().NotBe(null);
//            result.MembershipId.Should().Be(1);
//            result.AthleteId.Should().Be(1);
//        }

//        [Fact]
//        public async Task DeleteAsync_ShouldSoftDeleteTheMembership()
//        {
//            this.fixture.ResetDb();
//            _ = await this.SeedDatabaseAsync();

//            await this.service.DeleteByIdAsync(1);

//            var result = await this.data.Memberships.FirstAsync(m => m.Id == 1);
//            result.Should().NotBe(null);
//            result.IsDeleted.Should().Be(true);

//            var notDeletedEntity = await this.data.Memberships.FirstAsync(m => m.Id == 2);
//            notDeletedEntity.Should().NotBe(null);
//            notDeletedEntity.IsDeleted.Should().Be(false);
//        }

//        [Fact]
//        public async Task DeleteExpiredMembershipsAsync_ShouldSoftDeleteExpiredMemberships()
//        {
//            this.fixture.ResetDb();

//            var membership1 = new Membership
//            {
//                Id = 1,
//                IsMonthly = true,
//                StartDate = DateTime.UtcNow,
//                EndDate = DateTime.UtcNow,
//                TotalWorkoutsCount = null,
//                WorkoutsLeft = null,
//                Price = 96,
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                IsPending = true,
//            };

//            var athlete1 = new Athlete
//            {
//                Id = 1,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = membership1.Id,
//            };

//            var membership2 = new Membership
//            {
//                Id = 2,
//                IsMonthly = false,
//                StartDate = DateTime.UtcNow,
//                EndDate = null,
//                TotalWorkoutsCount = 12,
//                WorkoutsLeft = 12,
//                Price = 96,
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                IsPending = true,
//            };

//            var athlete2 = new Athlete
//            {
//                Id = 2,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = membership2.Id,
//            };

//            membership1.AthleteId = 1;
//            membership2.AthleteId = 2;

//            this.data.Memberships.AddRange(membership1, membership2);
//            this.data.Athletes.AddRange(athlete1, athlete2);
//            await this.data.SaveChangesAsync();

//            //await this.service.DeleteExpiredMembershipsAsync();

//            //var deleted = await this.data.Memberships.FirstAsync(m => m.Id == 1);
//            //var nonDeleted = await this.data.Memberships.FirstAsync(m => m.Id == 2);
//            //deleted.IsDeleted.Should().BeTrue();
//            //nonDeleted.IsDeleted.Should().BeFalse();
//        }

//        private async Task<Membership> SeedDatabaseAsync()
//        {
//            var membership1 = new Membership
//            {
//                Id = 1,
//                IsMonthly = false,
//                StartDate = DateTime.UtcNow,
//                EndDate = null,
//                TotalWorkoutsCount = 12,
//                WorkoutsLeft = 12,
//                Price = 96,
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                IsPending = true,
//            };

//            var membership2 = new Membership
//            {
//                Id = 2,
//                IsMonthly = false,
//                StartDate = DateTime.UtcNow,
//                EndDate = null,
//                TotalWorkoutsCount = 12,
//                WorkoutsLeft = 12,
//                Price = 96,
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                IsPending = false,
//            };

//            var athlete1 = new Athlete
//            {
//                Id = 1,
//                FirstName = "athl1",
//                LastName = "athl1",
//                PhoneNumber = "0000000001",
//                Email = null,
//                UserId = "test-user-id1",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = membership1.Id,
//            };

//            var athlete2 = new Athlete
//            {
//                Id = 2,
//                FirstName = "athl2",
//                LastName = "athl2",
//                PhoneNumber = "0000000000",
//                Email = null,
//                UserId = "test-user-id2",
//                CreatedOn = DateTime.UtcNow,
//                DeletedOn = null,
//                IsDeleted = false,
//                ModifiedOn = null,
//                MembershipId = membership2.Id,
//            };

//            membership1.AthleteId = 1;
//            membership2.AthleteId = 2;

//            await this.data.Athletes.AddRangeAsync(athlete1, athlete2);
//            await this.data.Memberships.AddRangeAsync(membership1, membership2);
//            await this.data.SaveChangesAsync();

//            return membership1;
//        }
//    }
//}
