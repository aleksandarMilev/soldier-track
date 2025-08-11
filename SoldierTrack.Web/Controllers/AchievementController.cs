namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Base;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Achievement;
    using Services.Achievement;
    using Services.Achievement.Models;
    using Services.Athlete;
    using Services.Exercise;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class AchievementController(
        IAchievementService achievementService,
        IExerciseService exerciseService,
        IAthleteService athleteService,
        IMapper mapper) : BaseController
    {
        private readonly IAchievementService achievementService = achievementService;
        private readonly IExerciseService exerciseService = exerciseService;
        private readonly IAthleteService athleteService = athleteService;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var models = await this.achievementService.GetAllByAthleteId(
                this.User.GetId()!,
                pageIndex,
                pageSize);

            return this.View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int exerciseId)
        {
            var athleteId = this.User.GetId();
            var exerciseName = await this.exerciseService.GetNameByIdAsync(exerciseId);

            if (await this.achievementService.AchievementIsAlreadyAddedAsync(
                exerciseId,
                athleteId!))
            {
                return this.BadRequest();
            }

            var model = new AchievementFormModel(
                athleteId!,
                exerciseId,
                exerciseName,
                DateTime.Now);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AchievementFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<AchievementServiceModel>(viewModel);
            await this.achievementService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = PRSuccessfullyAdded;

            return this.RedirectToAction(
                nameof(GetAll),
                new { athleteId = viewModel.AthleteId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.achievementService.GetByIdAsync(id);

            if (serviceModel is null)
            {
                return this.NotFound();
            }

            if (serviceModel.ExerciseIsDeleted)
            {
                this.TempData["FailureMessage"] = CustomExerciseDeleted;
                return this.RedirectToAction(nameof(GetAll));
            }

            if ((serviceModel.AthleteId == null) ||
                (serviceModel.AthleteId != this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            this.ViewBag.Id = id; 
            var viewModel = this.mapper.Map<AchievementFormModel>(serviceModel);

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            AchievementFormModel viewModel,
            int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<AchievementServiceModel>(viewModel);
            serviceModel.Id = id;
            await this.achievementService.EditAsync(serviceModel);
            
            this.TempData["SuccessMessage"] = AchievementEdited;

            return this.RedirectToAction(
                nameof(GetAll),
                new { athleteId = viewModel.AthleteId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int achievementId)
        {
            var athleteId = this.User.GetId()!; 
            await this.achievementService.DeleteAsync(achievementId, athleteId);

            this.TempData["SuccessMessage"] = AchievementDeleted;

            return this.RedirectToAction(
                nameof(GetAll),
                new { athleteId });
        }
    }
}
