namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Web.Areas.Administrator.Models;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;

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
        public IActionResult AddToWorkout(int athleteId)
        {
            var model = new AddAthleteToWorkoutViewModel() 
            { 
                AthleteId = athleteId,
                WorkoutDate = DateTime.Now 
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWorkout(AddAthleteToWorkoutViewModel model)
        {
            var workout = await this.workoutService.GetByDateAndTimeAsync(model.WorkoutDate, model.WorkoutTime);

            if (workout == null)
            {
                this.ModelState.AddModelError("", WorkoutNotFound);
                return this.View(model);
            }

            if (await this.athleteService.AthleteAlreadyJoinedByIdAsync(model.AthleteId, workout.Id))
            {
                this.ModelState.AddModelError("", string.Format(AthleteAlreadyJoined, workout.Title));
                return this.View(model);
            }

            await this.athleteService.JoinAsync(model.AthleteId, workout.Id);

            this.TempData["SuccessMessage"] = string.Format(string.Format(AdminAddedAthlete, workout.Title));
            return this.RedirectToAction("Details", "Workout", new { id = workout.Id, area = "" });
        }
    }
}
