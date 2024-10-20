namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Food;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Food;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;
    
    [Authorize]
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

        public async Task<IActionResult> GetAll(string? searchTerm, int pageIndex = 1, int pageSize = 2)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.SearchTerm = searchTerm;
            this.ViewBag.AthleteId = this.User.GetId();

            var model = await this.foodService.GetPageModelsAsync(searchTerm, pageIndex, pageSize);
            return this.View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new FoodFormModel() { AthleteId = this.User.GetId()! };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FoodFormModel viewModel)
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

            var athleteId = this.User.GetId();

            if (model.AthleteId != null && model.AthleteId == athleteId)
            {
                this.ViewBag.ShowButtons = true;
            }

            if (model.AthleteId == null || (model.AthleteId != null && model.AthleteId != athleteId))
            {
                this.ViewBag.AthleteId = athleteId;
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int foodId, string athleteId)
        {
            var serviceModel = await this.foodService.GetByIdAsync(foodId);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            if (serviceModel.AthleteId == null || serviceModel.AthleteId != athleteId)
            {
                return this.Unauthorized();
            }

            var viewModel = this.mapper.Map<FoodFormModel>(serviceModel);
            this.ViewBag.FoodId = foodId;
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FoodFormModel viewModel, int foodId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<FoodServiceModel>(viewModel);
            serviceModel.Id = foodId;
            await this.foodService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = FoodEdited;
            return this.RedirectToAction(nameof(Details), new { id = foodId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int foodId, string athleteId)
        {
            await this.foodService.DeleteAsync(foodId, athleteId);

            this.TempData["SuccessMessage"] = FoodDeleted;
            return this.RedirectToAction("MyDiary", "FoodDiary", new { athleteId });
        }
    }
}
