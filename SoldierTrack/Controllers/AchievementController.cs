namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Controllers.Base;
    using SoldierTrack.Web.Models.Achievement;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class AchievementController : BaseController
    {
        private readonly IAchievementService achievementService;
        private readonly IExerciseService exerciseService;
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public AchievementController(
            IAchievementService achievementService,
            IExerciseService exerciseService,
            IAthleteService athleteService,
            IMapper mapper)
        {
            this.achievementService = achievementService;
            this.exerciseService = exerciseService;
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var models = await this.achievementService.GetAllByAthleteIdAsync(this.User.GetId()!, pageIndex, pageSize);
            return this.View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int exerciseId)
        {
            var athleteId = this.User.GetId();
            var exerciseName = await this.exerciseService.GetNameByIdAsync(exerciseId);

            if (await this.achievementService.AchievementIsAlreadyAddedAsync(exerciseId, athleteId!))
            {
                return this.BadRequest();
            }

            var model = new AchievementFormModel(athleteId!, exerciseId, exerciseName, DateTime.Now);
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
            return this.RedirectToAction(nameof(GetAll), new { athleteId = viewModel.AthleteId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.achievementService.GetByIdAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            if (serviceModel.ExerciseIsDeleted)
            {
                this.TempData["FailureMessage"] = CustomExerciseDeleted;
                return this.RedirectToAction(nameof(GetAll));
            }

            if ((serviceModel.AthleteId == null) || (serviceModel.AthleteId != this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            this.ViewBag.Id = id; 
            var viewModel = this.mapper.Map<AchievementFormModel>(serviceModel);
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AchievementFormModel viewModel, int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<AchievementServiceModel>(viewModel);
            serviceModel.Id = id;
            await this.achievementService.EditAsync(serviceModel);
            
            this.TempData["SuccessMessage"] = AchievementEdited;
            return this.RedirectToAction(nameof(GetAll), new { athleteId = viewModel.AthleteId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int achievementId)
        {
            var athleteId = this.User.GetId()!; 
            await this.achievementService.DeleteAsync(achievementId, athleteId);

            this.TempData["SuccessMessage"] = AchievementDeleted;
            return this.RedirectToAction(nameof(GetAll), new { athleteId });
        }
    }
}
