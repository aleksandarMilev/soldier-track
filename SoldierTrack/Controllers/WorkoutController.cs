namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Workout;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class WorkoutController : Controller
    {
        private readonly IWorkoutService workoutService;

        public WorkoutController(IWorkoutService service)
            => this.workoutService = service;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(DateTime? date = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.workoutService.GetAllAsync(date, pageIndex, pageSize);
            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.workoutService.GetDetailsByIdAsync(id);
            return this.View(model);
        }
    }
}
