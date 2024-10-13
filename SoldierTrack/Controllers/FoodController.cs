namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Food;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Food;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [AthleteAuthorization]
    public class FoodController : Controller
    {
        private readonly IFoodService foodService;
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public FoodController(
            IFoodService foodService,
            IAthleteService athleteService,
            IMapper mapper)
        {
            this.foodService = foodService;
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var athleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);
            var model = new CreateFoodViewModel() { AthleteId = athleteId };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFoodViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<FoodServiceModel>(viewModel);
            var foodId = await this.foodService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = FoodCreated;
            return this.RedirectToAction(nameof(Details), new { id = foodId });
        }

        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.foodService.GetByIdAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            if (model.AthleteId != null && model.AthleteId != await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            return this.View(model);
        }


        //[HttpGet]
        //public async Task<IActionResult> Edit(int exerciseId, int athleteId)
        //{
        //    var serviceModel = await this.exerciseService.GetDetailsById(exerciseId);

        //    if (serviceModel == null)
        //    {
        //        return this.NotFound();
        //    }

        //    if (serviceModel.AthleteId == null || serviceModel.AthleteId != athleteId)
        //    {
        //        return this.Unauthorized();
        //    }

        //    var viewModel = this.mapper.Map<CreateExerciseViewModel>(serviceModel);
        //    this.ViewBag.ExerciseId = exerciseId;
        //    return this.View(viewModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(CreateExerciseViewModel viewModel, int exerciseId)
        //{
        //    if (!this.ModelState.IsValid)
        //    {
        //        return this.View(viewModel);
        //    }

        //    var serviceModel = this.mapper.Map<ExerciseDetailsServiceModel>(viewModel);
        //    serviceModel.Id = exerciseId;
        //    await this.exerciseService.EditAsync(serviceModel);

        //    this.TempData["SuccessMessage"] = ExerciseEdited;
        //    return this.RedirectToAction(nameof(Details), new { exerciseId });
        //}

        //[HttpPost]
        //public async Task<IActionResult> Delete(int exerciseId, int athleteId)
        //{
        //    await this.exerciseService.DeleteAsync(exerciseId, athleteId);

        //    this.TempData["SuccessMessage"] = ExerciseDeleted;
        //    return this.RedirectToAction(nameof(GetAll), new { includeMine = false });
        //}
    }
}
