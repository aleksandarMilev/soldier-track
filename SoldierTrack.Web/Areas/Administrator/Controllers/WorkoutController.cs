namespace SoldierTrack.Web.Areas.Administration.Controllers
{
    using Administrator.Controllers.Base;
    using Administrator.Models.Workout;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Services.Workout;
    using Services.Workout.Models;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class WorkoutController(
        IWorkoutService service,
        IMapper mapper) : BaseAdminController
    {
        private readonly IWorkoutService service = service;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new WorkoutFormModel()
            {
                Date = DateTime.Now
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkoutFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            if (await this.service.AnotherWorkoutExistsAtThisDateAndTime(
                viewModel.Date,
                viewModel.Time))
            {
                this.ModelState.AddModelError(
                    nameof(viewModel.Time),
                    string.Format(
                        WorkoutAlreadyListed,
                        DateTime.Today.Add(viewModel.Time).ToString("hh:mm tt")));

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutServiceModel>(viewModel);
            var workoutCreatedId = await this.service.Create(serviceModel);

            this.TempData["SuccessMessage"] = WorkoutCreated;

            return this.RedirectToAction(
                "Details",
                "Workout",
                new { area = "", id = workoutCreatedId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.service.GetModelById(id);

            if (serviceModel is null)
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

            if (await this.service.AnotherWorkoutExistsAtThisDateAndTime(
                viewModel.Date,
                viewModel.Time,
                id))
            {
                this.ModelState.AddModelError(
                    nameof(viewModel.Time),
                    string.Format(
                        WorkoutAlreadyListed,
                        DateTime.Today.Add(viewModel.Time).ToString("hh:mm tt")));

                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<WorkoutServiceModel>(viewModel);
            serviceModel.Id = id;
            var workoutEditedId = await this.service.Edit(serviceModel);

            this.TempData["SuccessMessage"] = WorkoutEdited;

            return this.RedirectToAction(
                "Details",
                "Workout",
                new { area = "", id = workoutEditedId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await this.service.Delete(id);
            this.TempData["SuccessMessage"] = WorkoutDeletedSuccessfully;

            return this.RedirectToAction(
                "GetAll",
                "Workout",
                new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(
            string athleteId,
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = athleteId;
            var model = await this.service.GetArchive(
                athleteId,
                pageIndex,
                pageSize);

            return this.View(
                "~/Views/Workout/GetArchive.cshtml",
                model);
        }
    }
}
