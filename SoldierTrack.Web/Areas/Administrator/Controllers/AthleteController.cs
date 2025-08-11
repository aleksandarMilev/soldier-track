namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using Base;
    using Microsoft.AspNetCore.Mvc;
    using Models.Athlete;
    using Services.Athlete;
    using Services.Workout;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class AthleteController(
        IAthleteService athleteService,
        IWorkoutService workoutService) : BaseAdminController
    {
        private readonly IAthleteService athleteService = athleteService;
        private readonly IWorkoutService workoutService = workoutService;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? searchTerm = null,
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewData[nameof(searchTerm)] = searchTerm;

            var models = await this.athleteService.GetPageModels(
                searchTerm,
                pageIndex,
                pageSize);

            return this.View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string athleteId)
        {
            var model = await this.athleteService.GetDetailsModelById(athleteId);

            if (model is null)
            {
                return this.NotFound();
            }

            return this.View("~/Views/Athlete/Details.cshtml", model);
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
            var workoutId = await this.workoutService.GetIdByDateAndTime(
                model.WorkoutDate,
                model.WorkoutTime);

            if (workoutId == null)
            {
                this.ModelState.AddModelError("", WorkoutNotFound);

                return this.View(model);
            }

            if (await this.athleteService.AthleteAlreadyJoinedById(
                model.AthleteId,
                workoutId.Value))
            {
                this.ModelState.AddModelError("", AthleteAlreadyJoined);

                return this.View(model);
            }

            if (await this.workoutService.WorkoutIsFull(workoutId.Value))
            {
                this.ModelState.AddModelError("", WorkoutIsFull);

                return this.View(model);
            }

            await this.athleteService.Join(
                model.AthleteId,
                workoutId.Value);

            this.TempData["SuccessMessage"] = AdminAddedAthlete;

            return this.RedirectToAction(
                "Details",
                "Workout",
                new { id = workoutId, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWorkout(
            string athleteId,
            int workoutId)
        {
            await this.athleteService.Leave(athleteId, workoutId);
            this.TempData["SuccessMessage"] = RemoveAthleteFromWorkout;

            return this.RedirectToAction(
                "Details",
                "Workout",
                new { id = workoutId, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string athleteId)
        {
            await this.athleteService.Delete(athleteId);
            this.TempData["SuccessMessage"] = AdminDeleteAthlete;

            return this.RedirectToAction(
                "Index",
                "Home",
                new { area = "" });
        }
    }
}
