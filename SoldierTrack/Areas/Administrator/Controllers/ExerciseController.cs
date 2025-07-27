namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using AutoMapper;
    using Base;
    using Microsoft.AspNetCore.Mvc;
    using Services.Exercise;
    using Services.Exercise.Models;
    using Web.Models.Exercise;

    using static Constants.MessageConstants;

    public class ExerciseController : BaseAdminController
    {
        private readonly IExerciseService exerciseService;
        private readonly IMapper mapper;

        public ExerciseController(IExerciseService exerciseService, IMapper mapper)
        {
            this.exerciseService = exerciseService;
            this.mapper = mapper;
        }

        public IActionResult Create() => this.View("~/Views/Exercise/Create.cshtml", new ExerciseFormModel());

        [HttpPost]
        public async Task<IActionResult> Create(ExerciseFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("~/Views/Exercise/Create.cshtml", viewModel);
            }

            if (await this.exerciseService.ExerciseWithThisNameExistsAsync(viewModel.Name))
            {
                this.ModelState.AddModelError(nameof(viewModel.Name), string.Format(ExerciseNameDuplicated, viewModel.Name));
                return this.View("~/Views/Exercise/Create.cshtml", viewModel);
            }

            var serviceModel = this.mapper.Map<ExerciseServiceModel>(viewModel);
            _ = await this.exerciseService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = ExerciseCreated;

            return this.RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
