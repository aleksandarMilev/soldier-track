namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using AutoMapper;
    using Base;
    using Microsoft.AspNetCore.Mvc;
    using Services.Exercise;
    using Services.Exercise.Models;
    using Web.Models.Exercise;

    using static Constants.MessageConstants;

    public class ExerciseController(
        IExerciseService service,
        IMapper mapper) : BaseAdminController
    {
        private readonly IExerciseService service = service;
        private readonly IMapper mapper = mapper;

        public IActionResult Create()
            => this.View(
                "~/Views/Exercise/Create.cshtml",
                new ExerciseFormModel());

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(
                    "~/Views/Exercise/Create.cshtml",
                    viewModel);
            }

            if (await this.service.ExerciseWithThisNameExists(viewModel.Name))
            {
                this.ModelState.AddModelError(
                    nameof(viewModel.Name),
                    string.Format(ExerciseNameDuplicated, viewModel.Name));

                return this.View("~/Views/Exercise/Create.cshtml", viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseServiceModel>(viewModel);
            _ = await this.service.Create(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseCreated;

            return this.RedirectToAction(
                "Index",
                "Home",
                new { area = "" });
        }
    }
}
