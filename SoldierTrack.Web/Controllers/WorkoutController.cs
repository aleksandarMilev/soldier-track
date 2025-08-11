namespace SoldierTrack.Web.Controllers
{
    using Base;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Services.Athlete;
    using Services.Membership;
    using Services.Workout;
    using Services.Workout.Models;

    using static Constants.WebConstants;

    public class WorkoutController(
        IWorkoutService workoutService,
        IAthleteService athleteService,
        IMembershipService membershipService,
        IMemoryCache cache) : BaseController
    {
        private readonly IWorkoutService workoutService = workoutService;
        private readonly IAthleteService athleteService = athleteService;
        private readonly IMembershipService membershipService = membershipService;
        private readonly IMemoryCache cache = cache;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            DateTime? date = null,
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.Date = date;

            var cacheKey = $"Workout_{date?.ToString("yyyyMMdd") ?? "All"}_{pageIndex}_{pageSize}";

            if (!this.cache.TryGetValue(cacheKey, out WorkoutPageServiceModel? model))
            {
                model = await this.workoutService.GetAll(
                    date,
                    pageIndex,
                    pageSize);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(WorkoutCacheDuration));

                this.cache.Set(
                    cacheKey,
                    model,
                    cacheOptions);
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.workoutService.GetArchive(
                this.User.GetId()!,
                pageIndex,
                pageSize);

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var athleteId = this.User.GetId()!;
            await this.membershipService.DeleteIfExpired(athleteId);

            var model = await this.workoutService.GetDetailsModelById(
                id,
                athleteId);

            if (model is null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }
    }
}
