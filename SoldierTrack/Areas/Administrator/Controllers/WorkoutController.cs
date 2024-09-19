namespace SoldierTrack.Web.Areas.Administration.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using SoldierTrack.Services.Category;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.Models;
    using SoldierTrack.Web.Areas.Administrator.Models.Workout;
    using SoldierTrack.Web.Areas.Administrator.Models.Workout.Base;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [Area(AdminRoleName)]
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
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateWorkoutViewModel() { Date = DateTime.Now };
            return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWorkoutViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutServiceModel>(viewModel);

            if (await this.workoutService.IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(serviceModel.Date, serviceModel.Time))
            {
                this.ModelState.AddModelError(nameof(serviceModel.Time), string.Format(WorkoutAlreadyListed, serviceModel.Time.ToString("hh\\:mm")));
                return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
            }

            await this.workoutService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = WorkoutCreated;
            return this.RedirectToAction("GetAll", "Workout", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.workoutService.GetByIdAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            //we should map it because the partial view we use for create/edit forms works with WorkoutBaseFormViewModel
            var viewModel = this.mapper.Map<EditWorkoutViewModel>(serviceModel);
            return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditWorkoutViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutDetailsServiceModel>(viewModel);

            if (await this.workoutService.IsAnotherWorkoutScheduledAtThisDateAndTimeAsync(serviceModel.Date, serviceModel.Time, serviceModel.Id))
            {
                this.ModelState.AddModelError(nameof(serviceModel.Time), string.Format(WorkoutAlreadyListed, serviceModel.Time.ToString("hh\\:mm")));
                return await this.ReturnWorkoutViewWithCategoriesLoaded(viewModel);
            }

            await this.workoutService.EditAsync(serviceModel);
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.workoutService.DeleteAsync(id);

            this.TempData["SuccessMessage"] = WorkoutDeletedSuccessfully;
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        private async Task<IActionResult> ReturnWorkoutViewWithCategoriesLoaded(WorkoutBaseFormModel viewModel)
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
