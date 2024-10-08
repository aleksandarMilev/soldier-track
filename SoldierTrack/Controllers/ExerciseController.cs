namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Web.Common.Extensions;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class ExerciseController : Controller
    {
        private readonly IExerciseService exerciseService;
        private readonly IAthleteService athleteService;

        public ExerciseController(IExerciseService exerciseService, IAthleteService athleteService)
        {
            this.exerciseService = exerciseService;
            this.athleteService = athleteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchTerm = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            var model = await this.exerciseService.GetPageModelsAsync(searchTerm, pageIndex, pageSize);
            return this.View(model);
        }


        public async Task<IActionResult> Details(int id)
        {
            var model = await this.exerciseService.GetDetailsById(id);
            return this.View(model);
        }
    }
}
