// namespace SoldierTrack.Tests.Achievements
// {
//     using AutoMapper;
//     using Common;
//     using Data;
//     using Data.Models;
//     using FluentAssertions;
//     using Microsoft.EntityFrameworkCore;
//     using Microsoft.Extensions.Logging;
//     using Moq;
//     using Services.Achievement;
//     using Services.Achievement.MapperProfile;
//     using SoldierTrack.Services.Achievement.Models;
//     using Xunit;

//     public class AchievementServiceTests
//     {
//         [Fact]
//         public async Task GetAllByAthleteId_ShouldReturnPaginatedModels()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var paginatedModels = await service.GetAllByAthleteId(
//                 "athlete-1",
//                 pageIndex: 1,
//                 pageSize: 2);

//             paginatedModels.Should().NotBeNull();
//             paginatedModels.Items.Should().HaveCount(2);

//             paginatedModels
//                 .Items
//                 .Select(i => i.ExerciseName)
//                 .Should()
//                 .ContainInOrder("Back Squat", "Bench Press");

//             paginatedModels.PageIndex.Should().Be(1);
//             paginatedModels.PageSize.Should().Be(2);
//             paginatedModels.TotalPages.Should().Be(2);
//         }

//         [Fact]
//         public async Task GetAllByAthleteId_ShouldReturnEmptyItems_WhenPageBeyondLast()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var paginatedModels = await service.GetAllByAthleteId(
//                 "athlete-1",
//                 pageIndex: 3,
//                 pageSize: 2);

//             paginatedModels.Should().NotBeNull();
//             paginatedModels.Items.Should().BeEmpty();
//             paginatedModels.TotalPages.Should().Be(2);
//         }

//         [Fact]
//         public async Task GetAchievementId_ShouldReturnId_WhenExists()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var id = await service.GetAchievementId(
//                 "athlete-1",
//                 exerciseId: 1);

//             id.Should().Be(101);
//         }

//         [Fact]
//         public async Task GetAchievementId_ShouldReturnNull_WhenDoesNotExist()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var id = await service.GetAchievementId(
//                 "athlete-1",
//                 exerciseId: 999);

//             id.Should().BeNull();
//         }

//         [Fact]
//         public async Task AchievementIsAlreadyAdded_ShouldReturnTrue_WhenExists()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var exists = await service.AchievementIsAlreadyAdded(
//                 2,
//                 "athlete-1");

//             exists.Should().BeTrue();
//         }

//         [Fact]
//         public async Task AchievementIsAlreadyAdded_ShouldReturnFalse_WhenDoesNotExist()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var exists = await service.AchievementIsAlreadyAdded(
//                 999,
//                 "athlete-1");

//             exists.Should().BeFalse();
//         }

//         [Fact]
//         public async Task GetById_ReturnsServiceModel()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var serviceModel = await service.GetById(103);

//             serviceModel.Should().NotBeNull();
//             serviceModel!.Id.Should().Be(103);
//             serviceModel.ExerciseName.Should().Be("Clean");
//             serviceModel.ExerciseIsDeleted.Should().BeTrue();
//         }

//         [Fact]
//         public async Task Create_ShouldPersist_AndAlso_ShouldCalculateOneRepMaxSmallReps()
//         {
//             using var data = CreateDb();
//             var exercise = new Exercise()
//             {
//                 Id = 5,
//                 Name = "Deadlift",
//                 IsDeleted = false,
//                 ImageUrl = "http://img/deadlift.jpg",
//                 Description = "Deadlift description"
//             };

//             await data.Exercises.AddAsync(exercise);
//             await data.SaveChangesAsync();

//             var mapper = CreateMapper();
//             var logger = new Mock<ILogger<AchievementService>>();
//             var service = CreateService(data, mapper, logger);

//             var model = new AchievementServiceModel()
//             {
//                 AthleteId = "athlete-9",
//                 ExerciseId = 5,
//                 WeightLifted = 120,
//                 Repetitions = 5
//             };

//             await service.Create(model);

//             var created = await data
//                 .Achievements
//                 .SingleAsync(a =>
//                     a.AthleteId == "athlete-9" &&
//                     a.ExerciseId == 5);

//             created.WeightLifted.Should().Be(120);
//             created.Repetitions.Should().Be(5);

//             var expected = 120 * Math.Pow(5, 0.1);
//             created
//                 .OneRepMax
//                 .Should()
//                 .BeApproximately(expected, precision: 1e-6);

//             logger.VerifyLogged(LogLevel.Information);
//         }

//         [Fact]
//         public async Task Edit_ShouldUpdateEntity_AndAlso_ShouldRecalculateOneRepMaxBigReps()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var logger = new Mock<ILogger<AchievementService>>();
//             var service = CreateService(data, mapper, logger);

//             var model = new AchievementServiceModel()
//             {
//                 Id = 101,
//                 AthleteId = "athlete-1",
//                 ExerciseId = 1,
//                 WeightLifted = 110,
//                 Repetitions = 12
//             };

//             await service.Edit(model);

//             var updated = await data.Achievements.SingleAsync(a => a.Id == 101);
//             updated.WeightLifted.Should().Be(110);
//             updated.Repetitions.Should().Be(12);

//             var expected = 110 * (1 + 0.0333 * 12);
//             updated
//                 .OneRepMax
//                 .Should()
//                 .BeApproximately(expected, 1e-6);

//             logger.VerifyLogged(LogLevel.Information);
//         }

//         [Fact]
//         public async Task Edit_ShouldUseSmallRepsFormula_WhenRepsEqual10()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var model = new AchievementServiceModel()
//             {
//                 Id = 101,
//                 AthleteId = "athlete-1",
//                 ExerciseId = 1,
//                 WeightLifted = 100,
//                 Repetitions = 10
//             };

//             await service.Edit(model);

//             var updated = await data.Achievements.SingleAsync(a => a.Id == 101);
//             var expected = 100 * Math.Pow(10, 0.1);
//             updated.OneRepMax.Should().BeApproximately(expected, 1e-6);
//         }

//         [Fact]
//         public async Task Edit_ShouldUseBigRepsFormula_WhenRepsEqual11()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             var model = new AchievementServiceModel()
//             {
//                 Id = 101,
//                 AthleteId = "athlete-1",
//                 ExerciseId = 1,
//                 WeightLifted = 100,
//                 Repetitions = 11
//             };

//             await service.Edit(model);

//             var updated = await data.Achievements.SingleAsync(a => a.Id == 101);
//             var expected = 100 * (1 + 0.0333 * 11);

//             updated.OneRepMax.Should().BeApproximately(expected, 1e-6);
//         }

//         [Fact]
//         public async Task Edit_ShouldThrowException_AndAlso_ShouldLogWarning_WhenDoesNotExist()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var logger = new Mock<ILogger<AchievementService>>();
//             var service = CreateService(data, mapper, logger);

//             var model = new AchievementServiceModel()
//             {
//                 Id = 999,
//                 AthleteId = "athlete-1",
//                 ExerciseId = 1,
//                 WeightLifted = 100,
//                 Repetitions = 3
//             };

//             var edit = async () => await service.Edit(model);

//             await edit
//                 .Should()
//                 .ThrowAsync<InvalidOperationException>()
//                 .WithMessage("Achievement not found!");

//             logger.VerifyLogged(LogLevel.Warning);
//         }

//         [Fact]
//         public async Task Delete_ShouldRemoveAchievement_AndAlso_ShouldLogInfo()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var logger = new Mock<ILogger<AchievementService>>();
//             var service = CreateService(data, mapper, logger);

//             var countBeforeDeletion = await data.Achievements.CountAsync(a => a.Id == 102);
//             countBeforeDeletion.Should().Be(1);

//             await service.Delete(
//                 achievementId: 102,
//                 athleteId: "athlete-ANY");

//             var countAfterDeletion = await data.Achievements.CountAsync(a => a.Id == 102);
//             countAfterDeletion.Should().Be(0);

//             logger.VerifyLogged(LogLevel.Information);
//         }

//         [Fact]
//         public async Task Delete_ShouldThrowException_AndAlso_ShouldLogWarning_WhenDoesNotExist()
//         {
//             using var data = CreateDb();

//             await SeedAsync(data);

//             var mapper = CreateMapper();
//             var logger = new Mock<ILogger<AchievementService>>();
//             var service = CreateService(data, mapper, logger);

//             var delete = async () => await service.Delete(
//                 achievementId: 555,
//                 athleteId: "athlete-1");

//             await delete
//                 .Should()
//                 .ThrowAsync<InvalidOperationException>()
//                 .WithMessage("Achievement not found!");

//             logger.VerifyLogged(LogLevel.Warning);
//         }

//  [Fact]
//         public async Task GetRankings_ShouldReturnTop10Rankings()
//         {
//             using var data = CreateDb();

//             var mapper = CreateMapper();
//             var service = CreateService(data, mapper);

//             // Seed athletes with required properties
//             var athletes = Enumerable
//                 .Range(1, 12)
//                 .Select(i => new Athlete
//                 {
//                     Id = $"athlete-{i}",
//                     FirstName = $"First{i}",
//                     LastName = $"Last{i}"
//                 })
//                 .ToList();

//             await data.AddRangeAsync(athletes);

//             var exercise = new Exercise()
//             {
//                 Id = 10,
//                 Name = "Snatch",
//                 IsDeleted = false,
//                 ImageUrl = "http://foo.jpg",
//                 Description = "Snatch description"
//             };

//             await data.Exercises.AddAsync(exercise);
//             await data.SaveChangesAsync();

//             for (int i = 1; i <= 12; i++)
//             {
//                 await service.Create(new AchievementServiceModel()
//                 {
//                     AthleteId = $"athlete-{i}",
//                     ExerciseId = 10,
//                     WeightLifted = 50 + i,
//                     Repetitions = i % 12 + 1
//                 });
//             }

//             var rankings = (await service.GetRankings(10)).ToList();

//             rankings.Should().HaveCount(10);
//             rankings.Select(r => r.OneRepMax).Should().BeInDescendingOrder();
//         }

//         private static ApplicationDbContext CreateDb(string? name = null)
//         {
//             var dbName = name ?? $"SoldierTrack-AchievementService-Tests-{Guid.NewGuid()}";
//             var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                 .UseInMemoryDatabase(dbName)
//                 .EnableSensitiveDataLogging()
//                 .Options;

//             return new ApplicationDbContext(options);
//         }

//         private static IMapper CreateMapper()
//             => new MapperConfiguration(cfg =>
//             {
//                 cfg.AddProfile(new AchievementProfile());
//             })
//             .CreateMapper();

//         private static AchievementService CreateService(
//             ApplicationDbContext data,
//             IMapper mapper,
//             Mock<ILogger<AchievementService>>? loggerMock = null)
//             => new(
//                 data,
//                 loggerMock?.Object ?? Mock.Of<ILogger<AchievementService>>(),
//                 mapper);

//         private static async Task SeedAsync(ApplicationDbContext data)
//         {
//             var backSquat = new Exercise()
//             {
//                 Id = 1,
//                 Name = "Back Squat",
//                 IsDeleted = false,
//                 ImageUrl = "http://baz.com",
//                 Description = "Back squat description"
//             };

//             var bench = new Exercise()
//             {
//                 Id = 2,
//                 Name = "Bench Press",
//                 IsDeleted = false,
//                 ImageUrl = "http://bar.com",
//                 Description = "Bench press description"
//             };

//             var clean = new Exercise()
//             {
//                 Id = 3,
//                 Name = "Clean",
//                 IsDeleted = true,
//                 ImageUrl = "http://foo.com",
//                 Description = "Clean description"
//             };

//             var achievements = new List<Achievement>
//             {
//                 new()
//                 {
//                     Id = 101,
//                     AthleteId = "athlete-1",
//                     ExerciseId = backSquat.Id,
//                     Exercise = backSquat,
//                     WeightLifted = 100,
//                     Repetitions = 5,
//                     OneRepMax = 0
//                 },
//                 new()
//                 {
//                     Id = 102,
//                     AthleteId = "athlete-1",
//                     ExerciseId = bench.Id,
//                     Exercise = bench,
//                     WeightLifted = 80,
//                     Repetitions = 3,
//                     OneRepMax = 0
//                 },
//                 new()
//                 {
//                     Id = 103,
//                     AthleteId = "athlete-1",
//                     ExerciseId = clean.Id,
//                     Exercise = clean,
//                     WeightLifted = 70,
//                     Repetitions = 12,
//                     OneRepMax = 0
//                 },
//                 new()
//                 {
//                     Id = 201,
//                     AthleteId = "athlete-2",
//                     ExerciseId = bench.Id,
//                     Exercise = bench,
//                     WeightLifted = 60,
//                     Repetitions = 8,
//                     OneRepMax = 0
//                 }
//             };

//             await data.Exercises.AddRangeAsync(backSquat, bench, clean);
//             await data.Achievements.AddRangeAsync(achievements);
//             await data.SaveChangesAsync();
//         }
//     }
// }
