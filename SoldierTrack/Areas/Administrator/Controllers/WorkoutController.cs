namespace SoldierTrack.Web.Areas.Administration.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Workout;
    using SoldierTrack.Services.Workout.Models;
    using SoldierTrack.Web.Areas.Administrator.Models.Workout;
    using SoldierTrack.Web.Areas.Administrator.Controllers.Base;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    public class WorkoutController : BaseAdminController
    {
        private readonly IWorkoutService workoutService;
        private readonly IMapper mapper;

        public WorkoutController(IWorkoutService workoutService, IMapper mapper)
        {
            this.workoutService = workoutService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new WorkoutFormModel() { Date = DateTime.Now };
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkoutFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            if (await this.workoutService.AnotherWorkoutExistsAtThisDateAndTimeAsync(viewModel.Date, viewModel.Time))
            {
                this.ModelState.AddModelError(
                    nameof(viewModel.Time),
                    string.Format(WorkoutAlreadyListed, DateTime.Today.Add(viewModel.Time).ToString("hh:mm tt")));

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutServiceModel>(viewModel);
            var workoutCreatedId = await this.workoutService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = WorkoutCreated;
            return this.RedirectToAction("Details", "Workout", new { area = "", id = workoutCreatedId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.workoutService.GetModelByIdAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            this.ViewBag.Id = id;
            var viewModel = this.mapper.Map<WorkoutFormModel>(serviceModel);
            viewModel.Time = viewModel.Date.ToLocalTime().TimeOfDay;

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WorkoutFormModel viewModel, int id)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            if (await this.workoutService.AnotherWorkoutExistsAtThisDateAndTimeAsync(viewModel.Date, viewModel.Time, id))
            {
                this.ModelState.AddModelError(
                    nameof(viewModel.Time),
                    string.Format(WorkoutAlreadyListed, DateTime.Today.Add(viewModel.Time).ToString("hh:mm tt")));

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutServiceModel>(viewModel);
            serviceModel.Id = id;
            var workoutEditedId = await this.workoutService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = WorkoutEdited;
            return this.RedirectToAction("Details", "Workout", new { area = "", id = workoutEditedId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.workoutService.DeleteAsync(id);

            this.TempData["SuccessMessage"] = WorkoutDeletedSuccessfully;
            return this.RedirectToAction("GetAll", "Workout", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(string athleteId, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = athleteId;
            var model = await this.workoutService.GetArchiveAsync(athleteId, pageIndex, pageSize);

            return this.View("~/Views/Workout/GetArchive.cshtml", model);
        }
    }
}
