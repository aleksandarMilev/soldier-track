namespace SoldierTrack.Web.Areas.Administration.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using SoldierTrack.Services.Category;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.Models;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using SoldierTrack.Web.Areas.Administrator.Models.Workout;

    [Area("Administrator")]
    [Authorize(Roles = AdminRoleName)]
    public class WorkoutController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IWorkoutService workoutService;
        private readonly IMapper mapper;

        public WorkoutController(
            ICategoryService categoryService,
            IWorkoutService workoutService,
            IMapper mapper)
        {
            this.categoryService = categoryService;
            this.workoutService = workoutService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> CreateWorkout()
        {
            var viewModel = new FormWorkoutViewModel() { Date = DateTime.Now };
            return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkout(FormWorkoutViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutServiceModel>(viewModel);

            if (await this.workoutService.IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(serviceModel))
            {
                this.ModelState.AddModelError("Time", string.Format(WorkoutAlreadyListed, serviceModel.Time.ToString("hh\\:mm")));
                return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
            }

            await this.workoutService.CreateAsync(serviceModel);
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        private async Task<IActionResult> ReturnWorkoutViewWithCategoriesLoaded(FormWorkoutViewModel viewModel)
        {
            var categories = await this.categoryService.GetAllAsync();

            var categorySelectList = categories
               .Select(c => new SelectListItem()
               {
                   Value = c.Id.ToString(),
                   Text = c.Name
               });

            viewModel.Categories = categorySelectList;
            return this.View(viewModel);
        }
    }
}
