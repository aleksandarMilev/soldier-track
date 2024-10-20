namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Services.Exercise.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Exercise;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [Authorize]
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
        public async Task<IActionResult> GetAll(bool includeMine, string? searchTerm = null, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var athleteId = this.User.GetId();
            this.ViewBag.AthleteId = athleteId;

            var model = await this.exerciseService.GetPageModelsAsync(searchTerm, athleteId!, includeMine, pageIndex, pageSize);
            this.ViewData[nameof(includeMine)] = includeMine.ToString().ToLower();
            this.ViewData[nameof(searchTerm)] = searchTerm;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int exerciseId)
        {
            var model = await this.exerciseService.GetDetailsById(exerciseId);

            if (model == null)
            {
                return this.NotFound();
            }

            var athleteId = this.User.GetId();
            var achievementId = await this.achievementService.GetAchievementIdAsync(athleteId!, exerciseId);

            if (achievementId == null)
            {
                this.ViewBag.ShowCreateButton = true;
            }
            else
            {
                //current athlete has already added the exercise as achievement, we take its id because we need it 
                this.ViewBag.AchievementId = achievementId;
            }

            if (model.AthleteId == null)
            {
                //if exercise is not custom, we should get the current athleteId in the view bag because we will need it in the view
                this.ViewBag.AthleteId = athleteId; 

            }

            if (model.AthleteId != null && model.AthleteId == athleteId)
            {
                //exercise is custom and the current athlete is the creator
                this.ViewBag.ShowEditButton = true;
                this.ViewBag.ShowDeleteButton = true;
            }

            return this.View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ExerciseFormModel()
            {
                AthleteId = this.User.GetId()!
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseFormModel viewModel)
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

        [HttpGet]
        public async Task<IActionResult> Edit(int exerciseId, string athleteId)
        {
            var serviceModel = await this.exerciseService.GetDetailsById(exerciseId);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            if (serviceModel.AthleteId == null || serviceModel.AthleteId != athleteId)
            {
                return this.Unauthorized();
            }

            var viewModel = this.mapper.Map<ExerciseFormModel>(serviceModel);
            this.ViewBag.ExerciseId = exerciseId; 
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ExerciseFormModel viewModel, int exerciseId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseDetailsServiceModel>(viewModel);
            serviceModel.Id = exerciseId;
            await this.exerciseService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseEdited;
            return this.RedirectToAction(nameof(Details), new { exerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int exerciseId, string athleteId)
        {
            await this.exerciseService.DeleteAsync(exerciseId, athleteId);
            
            this.TempData["SuccessMessage"] = ExerciseDeleted;
            return this.RedirectToAction(nameof(GetAll), new { includeMine = false });
        }
    }
}
