namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Controllers.Base;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class WorkoutController : BaseController
    {
        private readonly IWorkoutService workoutService;
        private readonly IAthleteService athleteService;
        private readonly IMembershipService membershipService;
        private readonly IMemoryCache cache;

        public WorkoutController(
            IWorkoutService workoutService,
            IAthleteService athleteService,
            IMembershipService membershipService,
            IMemoryCache cache)
        {
            this.workoutService = workoutService;
            this.athleteService = athleteService;
            this.membershipService = membershipService;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(DateTime? date = null, int pageIndex = DefaultPageIndex, int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.Date = date;

            var cacheKey = $"Workout_{date?.ToString("yyyyMMdd") ?? "All"}_{pageIndex}_{pageSize}";

            if (!this.cache.TryGetValue(cacheKey, out WorkoutPageServiceModel? model))
            {
                model = await this.workoutService.GetAllAsync(date, pageIndex, pageSize);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(WorkoutCacheDuration));
                this.cache.Set(cacheKey, model, cacheOptions);
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(int pageIndex = DefaultPageIndex, int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.workoutService.GetArchiveAsync(this.User.GetId()!, pageIndex, pageSize);
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var athleteId = this.User.GetId()!;
            await this.membershipService.DeleteIfExpiredAsync(athleteId);

            var model = await this.workoutService.GetDetailsModelByIdAsync(id, athleteId);

            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }
    }
}
