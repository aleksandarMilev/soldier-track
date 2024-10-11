namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Services.Exercise.Models;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Exercise;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [AthleteAuthorization]
    public class ExerciseController : Controller
    {
        private readonly IExerciseService exerciseService;
        private readonly IAthleteService athleteService;
        private readonly IAchievementService achievementService;
        private readonly IMapper mapper;

        public ExerciseController(
            IExerciseService exerciseService,
            IAthleteService athleteService,
            IAchievementService achievementService,
            IMapper mapper)
        {
            this.exerciseService = exerciseService;
            this.athleteService = athleteService;
            this.achievementService = achievementService;
            this.mapper = mapper;
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

        [HttpGet]
        public async Task<IActionResult> Details(int exerciseId)
        {
            var model = await this.exerciseService.GetDetailsById(exerciseId);
            var athleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);

            var achievementId = await this.achievementService.GetAchievementIdAsync(athleteId.Value, exerciseId);
            if (achievementId == null)
            {
                this.ViewBag.ShowCreateButton = true;
                this.ViewBag.AthleteId = athleteId;
            }
            else
            {
                this.ViewBag.AchievementId = achievementId;
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var athleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            var model = new CreateExerciseViewModel()
            {
                AthleteId = athleteId.Value
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExerciseViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseDetailsServiceModel>(viewModel);
            var exerciseId = await this.exerciseService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseCreated;
            return this.RedirectToAction(nameof(Details), new { exerciseId });
        }
    }
}
