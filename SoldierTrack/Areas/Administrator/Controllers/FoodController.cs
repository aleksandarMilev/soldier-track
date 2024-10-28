namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Food.Models;
    using SoldierTrack.Services.Food;
    using SoldierTrack.Web.Models.Food;
    using SoldierTrack.Web.Areas.Administrator.Controllers.Base;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    public class FoodController : BaseAdminController
    {
        private readonly IFoodService foodService;
        private readonly IMapper mapper;

        public FoodController(IFoodService foodService, IMapper mapper)
        {
            this.foodService = foodService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create() => this.View("~/Views/Food/Create.cshtml", new FoodFormModel());

        [HttpPost]
        public async Task<IActionResult> Create(FoodFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("~/Views/Food/Create.cshtml", viewModel);
            }

            var serviceModel = this.mapper.Map<FoodServiceModel>(viewModel);
            _ = await this.foodService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = FoodCreated;
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
