namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Web.Common.Extensions;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [Authorize]
    public class WorkoutController : Controller
    {
        private readonly IWorkoutService workoutService;
        private readonly IAthleteService athleteService;
        private readonly IMembershipService membershipService;

        public WorkoutController(
            IWorkoutService workoutService,
            IAthleteService athleteService,
            IMembershipService membershipService)
        {
            this.workoutService = workoutService;
            this.athleteService = athleteService;
            this.membershipService = membershipService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(DateTime? date = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.Date = date;
            var model = await this.workoutService.GetAllAsync(date, pageIndex, pageSize);
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

        [HttpGet]
        public async Task<IActionResult> GetArchive(string athleteId, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            if (this.User.GetId() != athleteId)
            {
                return this.Unauthorized();
            }

            this.ViewBag.AthleteId = athleteId;
            var model = await this.workoutService.GetArchiveAsync(athleteId, pageIndex, pageSize);
            return this.View(model);
        }
    }
}
