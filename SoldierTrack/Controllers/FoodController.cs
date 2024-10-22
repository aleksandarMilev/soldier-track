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

        public async Task<IActionResult> GetAll([FromQuery] FoodSearchParams searchParams)
        {
            searchParams.PageSize = Math.Min(searchParams.PageSize, MaxPageSize);
            searchParams.PageSize = Math.Max(searchParams.PageSize, MinPageSize);

            var model = await this.foodService.GetPageModelsAsync(searchParams, this.User.GetId()!, this.User.IsAdmin());

            this.ViewData[nameof(searchParams.IncludeMine)] = searchParams.IncludeMine.ToString().ToLower();
            this.ViewData[nameof(searchParams.IncludeCustom)] = searchParams.IncludeCustom.ToString().ToLower();
            this.ViewData[nameof(searchParams.SearchTerm)] = searchParams.SearchTerm;

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

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int foodId)
        {
            var serviceModel = await this.foodService.GetByIdAsync(foodId);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            if (serviceModel.AthleteId == null || serviceModel.AthleteId != this.User.GetId())
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
        public async Task<IActionResult> Delete(int foodId)
        {
            var athleteId = this.User.GetId();
            await this.foodService.DeleteAsync(foodId, athleteId!, this.User.IsAdmin());

            this.TempData["SuccessMessage"] = FoodDeleted;
            return this.RedirectToAction(nameof(GetAll));
        }
    }
}
