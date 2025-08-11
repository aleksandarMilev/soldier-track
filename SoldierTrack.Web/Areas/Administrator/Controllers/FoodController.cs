namespace SoldierTrack.Web.Areas.Administrator.Controllers
{
    using AutoMapper;
    using Base;
    using Microsoft.AspNetCore.Mvc;
    using Services.Food;
    using Services.Food.Models;
    using Web.Models.Food;

    using static Constants.MessageConstants;

    public class FoodController(
        IFoodService service,
        IMapper mapper) : BaseAdminController
    {
        private readonly IFoodService service = service;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        public IActionResult Create() 
            => this.View(
                "~/Views/Food/Create.cshtml",
                new FoodFormModel());

        [HttpPost]
        public async Task<IActionResult> Create(FoodFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(
                    "~/Views/Food/Create.cshtml",
                    viewModel);
            }

            var serviceModel = this.mapper.Map<FoodServiceModel>(viewModel);
            _ = await this.service.Create(serviceModel);

            this.TempData["SuccessMessage"] = FoodCreated;

            return this.RedirectToAction(
                "Index",
                "Home",
                new { area = "" });
        }
    }
}
