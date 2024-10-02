namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using SoldierTrack.Services.Achievement;
    using SoldierTrack.Services.Achievement.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Exercise;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Achievement;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [AthleteAuthorization]
    public class AchievementController : Controller
    {
        private readonly IAchievementService achievementService;
        private readonly IExcerciseService exerciseService;
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public AchievementController(
            IAchievementService achievementService,
            IExcerciseService exerciseService,
            IAthleteService athleteService,
            IMapper mapper)
        {
            this.achievementService = achievementService;
            this.exerciseService = exerciseService;
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var athleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            var models = await this.achievementService.GetAllByAthleteIdAsync(athleteId);
            return this.View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await this.GetFormViewModel();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAchievementViewModel viewModel)
        {
            if (!this.ModelState.IsValid || await this.achievementService.AcheivementIsAlreadyAdded(viewModel.ExerciseId, viewModel.AthleteId))
            {
                if (await this.achievementService.AcheivementIsAlreadyAdded(viewModel.ExerciseId, viewModel.AthleteId))
                {
                    this.ModelState.AddModelError("", AchievementAlreadyAdded);
                }

                var updatedViewModel = await this.GetFormViewModel(viewModel);
                return this.View(updatedViewModel);
            }

            var serviceModel = this.mapper.Map<AchievementServiceModel>(viewModel);
            await this.achievementService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = PRSuccessfullyAdded;
            return this.RedirectToAction(nameof(GetAll));
        }

        private async Task<CreateAchievementViewModel> GetFormViewModel(CreateAchievementViewModel? viewModel = null)
        {
            var athleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            var exercises = await this.exerciseService.GetAllAsycn();

            var exerciseSelectList = exercises
                .Select(c => new SelectListItem()
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

            if (viewModel == null)
            {
                viewModel = new CreateAchievementViewModel
                {
                    AthleteId = athleteId,
                    DateAchieved = DateTime.Now.Date,
                };
            }

            viewModel.Exercises = exerciseSelectList;
            viewModel.AthleteId = athleteId;

            return viewModel;
        }
    }
}
