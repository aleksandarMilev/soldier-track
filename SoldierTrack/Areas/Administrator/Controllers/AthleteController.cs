namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Workout;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using SoldierTrack.Web.Areas.Administrator.Models.Athlete;

    [Area(AdminRoleName)]
    [Authorize(Roles = AdminRoleName)]
    public class AthleteController : Controller
    {
        private readonly IAthleteService athleteService;
        private readonly IWorkoutService workoutService;

        public AthleteController(IAthleteService athleteService, IWorkoutService workoutService)
        {
            this.athleteService = athleteService;
            this.workoutService = workoutService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchTerm = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.athleteService.GetPageModelsAsync(searchTerm, pageIndex, pageSize);
            return this.View(model);
        }

        [HttpGet]
        public IActionResult AddToWorkout(string athleteId)
        {
            var model = new AddAthleteToWorkoutModel()
            {
                AthleteId = athleteId,
                WorkoutDate = DateTime.Now,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWorkout(AddAthleteToWorkoutModel model)
        {
            var workoutId = await this.workoutService.GetIdByDateAndTimeAsync(model.WorkoutDate, model.WorkoutTime);

            if (workoutId == null)
            {
                this.ModelState.AddModelError("", WorkoutNotFound);
                return this.View(model);
            }

            if (await this.athleteService.AthleteAlreadyJoinedByIdAsync(model.AthleteId, workoutId.Value))
            {
                this.ModelState.AddModelError("", AthleteAlreadyJoined);
                return this.View(model);
            }

            if (await this.workoutService.WorkoutIsFull(workoutId.Value))
            {
                this.ModelState.AddModelError("", WorkoutIsFull);
                return this.View(model);
            }

            await this.athleteService.JoinAsync(model.AthleteId, workoutId.Value);

            this.TempData["SuccessMessage"] = AdminAddedAthlete;
            return this.RedirectToAction("Details", "Workout", new { id = workoutId, area = "" });
        }
    }
}
