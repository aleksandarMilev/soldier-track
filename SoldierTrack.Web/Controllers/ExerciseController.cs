namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Controllers.Base;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Exercise;
    using Services.Achievement;
    using Services.Athlete;
    using Services.Exercise;
    using Services.Exercise.Models;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class ExerciseController(
        IExerciseService exerciseService,
        IAthleteService athleteService,
        IAchievementService achievementService,
        IMapper mapper) : BaseController
    {
        private readonly IExerciseService exerciseService = exerciseService;
        private readonly IAthleteService athleteService = athleteService;
        private readonly IAchievementService achievementService = achievementService;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ExerciseSearchParams searchParams)
        {
            searchParams.PageSize = Math.Min(searchParams.PageSize, MaxPageSize);
            searchParams.PageSize = Math.Max(searchParams.PageSize, MinPageSize);

            var model = await this.exerciseService.GetPageModels(
                searchParams,
                this.User.GetId()!,
                this.User.IsAdmin());

            this.ViewData[nameof(searchParams.IncludeMine)] = searchParams
                .IncludeMine
                .ToString()
                .ToLower();

            this.ViewData[nameof(searchParams.IncludeCustom)] = searchParams
                .IncludeCustom
                .ToString()
                .ToLower();

            this.ViewData[nameof(searchParams.SearchTerm)] = searchParams.SearchTerm;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var athleteId = this.User.GetId();

            if (await this.exerciseService.ExerciseLimitReached(athleteId!))
            {
                this.TempData["FailureMessage"] = MaxExercisesLimit;
                return this.RedirectToAction(
                    "GetAll",
                    "Achievement");
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

            if (await this.exerciseService.ExerciseWithThisNameExists(viewModel.Name))
            {
                this.ModelState.AddModelError(
                    nameof(viewModel.Name),
                    string.Format(ExerciseNameDuplicated, viewModel.Name));

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseServiceModel>(viewModel);
            var exerciseId = await this.exerciseService.Create(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseCreated;

            return this.RedirectToAction(
                nameof(this.Details),
                new { exerciseId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int exerciseId)
        {
            var model = await this.exerciseService.GetDetailsById(
                exerciseId,
                this.User.GetId()!,
                this.User.IsAdmin());

            if (model is null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int exerciseId)
        {
            var serviceModel = await this.exerciseService.GetById(exerciseId);

            if (serviceModel is null)
            {
                return this.NotFound();
            }

            var userIsUnauthorized =
                serviceModel.AthleteId is null &&
                !this.User.IsAdmin() &&
                serviceModel.AthleteId != this.User.GetId()!;

            if (userIsUnauthorized)
            {
                return this.Unauthorized();
            }

            var viewModel = this.mapper.Map<ExerciseFormModel>(serviceModel);
            this.ViewBag.ExerciseId = exerciseId; 

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            ExerciseFormModel viewModel,
            int exerciseId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseServiceModel>(viewModel);
            serviceModel.Id = exerciseId;
            await this.exerciseService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseEdited;

            return this.RedirectToAction(
                nameof(this.Details),
                new { exerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int exerciseId)
        {
            await this.exerciseService.Delete(
                exerciseId,
                this.User.GetId()!,
                this.User.IsAdmin());
            
            this.TempData["SuccessMessage"] = ExerciseDeleted;

            return this.RedirectToAction(nameof(this.GetAll));
        }
    }
}
