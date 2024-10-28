namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Services.Exercise.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Controllers.Base;
    using SoldierTrack.Web.Models.Exercise;
    
    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class ExerciseController : BaseController
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
        public async Task<IActionResult> GetAll([FromQuery] ExerciseSearchParams searchParams)
        {
            searchParams.PageSize = Math.Min(searchParams.PageSize, MaxPageSize);
            searchParams.PageSize = Math.Max(searchParams.PageSize, MinPageSize);

            var model = await this.exerciseService.GetPageModelsAsync(searchParams, this.User.GetId()!, this.User.IsAdmin());

            this.ViewData[nameof(searchParams.IncludeMine)] = searchParams.IncludeMine.ToString().ToLower();
            this.ViewData[nameof(searchParams.IncludeCustom)] = searchParams.IncludeCustom.ToString().ToLower();
            this.ViewData[nameof(searchParams.SearchTerm)] = searchParams.SearchTerm;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var athleteId = this.User.GetId();

            if (await this.exerciseService.ExerciseLimitReachedAsync(athleteId!))
            {
                this.TempData["FailureMessage"] = MaxExercisesLimit;
                return this.RedirectToAction("GetAll", "Achievement");
            }

            var model = new ExerciseFormModel()
            {
                AthleteId = athleteId!
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

            if (await this.exerciseService.ExerciseWithThisNameExistsAsync(viewModel.Name))
            {
                this.ModelState.AddModelError(nameof(viewModel.Name), string.Format(ExerciseNameDuplicated, viewModel.Name));
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseServiceModel>(viewModel);
            var exerciseId = await this.exerciseService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseCreated;
            return this.RedirectToAction(nameof(Details), new { exerciseId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int exerciseId)
        {
            var model = await this.exerciseService.GetDetailsById(exerciseId, this.User.GetId()!, this.User.IsAdmin());

            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int exerciseId)
        {
            var serviceModel = await this.exerciseService.GetByIdAsync(exerciseId);
            if (serviceModel == null)
            {
                return this.NotFound();
            }

            if ((serviceModel.AthleteId == null && !this.User.IsAdmin()) && 
                 serviceModel.AthleteId != this.User.GetId()!)
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

            var serviceModel = this.mapper.Map<ExerciseServiceModel>(viewModel);
            serviceModel.Id = exerciseId;
            await this.exerciseService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseEdited;
            return this.RedirectToAction(nameof(Details), new { exerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int exerciseId)
        {
            await this.exerciseService.DeleteAsync(exerciseId, this.User.GetId()!, this.User.IsAdmin());
            
            this.TempData["SuccessMessage"] = ExerciseDeleted;
            return this.RedirectToAction(nameof(GetAll));
        }
    }
}
