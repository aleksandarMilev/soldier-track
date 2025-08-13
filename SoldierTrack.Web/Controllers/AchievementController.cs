namespace SoldierTrack.Web.Controllers
{
    using Base;
    using Mapping;
    using Microsoft.AspNetCore.Mvc;
    using Models.Achievement;
    using Services.Achievement;
    using Services.Athlete;
    using Services.Exercise;
    using WebServices.CurrentUser;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class AchievementController(
        IAchievementService achievementService,
        IExerciseService exerciseService,
        IAthleteService athleteService,
        ICurrentUserService userService) : BaseController
    {
        private readonly IAchievementService achievementService = achievementService;
        private readonly IExerciseService exerciseService = exerciseService;
        private readonly IAthleteService athleteService = athleteService;
        private readonly ICurrentUserService userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);
            pageIndex = Math.Max(pageIndex, MinPageIndex);

            var models = await this.achievementService.GetAllByAthleteId(
                this.userService.GetId()!,
                pageIndex,
                pageSize);

            return this.View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int exerciseId)
        {
            var exerciseName = await this.exerciseService.GetNameById(exerciseId);

            if (await this.achievementService.AchievementIsAlreadyAdded(
                exerciseId,
                this.userService.GetId()!))
            {
                return this.BadRequest();
            }

            var model = new AchievementFormModel(
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

            await this.achievementService.Create(
                viewModel.ToServiceModel(),
                this.userService.GetId()!);

            this.TempData["SuccessMessage"] = PRSuccessfullyAdded;

            return this.RedirectToAction(nameof(this.GetAll));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.achievementService.GetById(id);

            if (serviceModel is null)
            {
                return this.NotFound();
            }

            if (serviceModel.ExerciseIsDeleted)
            {
                this.TempData["FailureMessage"] = CustomExerciseDeleted;
                return this.RedirectToAction(nameof(this.GetAll));
            }

            var userIsUnauthorized =
                serviceModel.AthleteId is null ||
                serviceModel.AthleteId != this.userService.GetId();

            if (userIsUnauthorized)
            {
                return this.Unauthorized();
            }

            this.ViewBag.Id = id; 

            return this.View(serviceModel.ToViewModel());
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

            var serviceModel = viewModel.ToServiceModel();
            serviceModel.Id = id;
            
            await this.achievementService.Edit(
                serviceModel,
                this.userService.GetId()!);
            
            this.TempData["SuccessMessage"] = AchievementEdited;

            return this.RedirectToAction(nameof(this.GetAll));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int achievementId)
        {
            await this.achievementService.Delete(
                achievementId,
                this.userService.GetId()!);

            this.TempData["SuccessMessage"] = AchievementDeleted;

            return this.RedirectToAction(nameof(this.GetAll));
        }
    }
}
