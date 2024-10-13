namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Food;
    using SoldierTrack.Services.FoodDiary;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;

    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [AthleteAuthorization]
    public class FoodDiaryController : Controller
    {
        private readonly IFoodDiaryService foodDiaryService;
        private readonly IAthleteService athleteService;
        private readonly IFoodService foodService;

        public FoodDiaryController(
            IFoodDiaryService foodDiaryService,
            IAthleteService athleteService,
            IFoodService foodService)
        {
            this.foodDiaryService = foodDiaryService;
            this.athleteService = athleteService;
            this.foodService = foodService;
        }

        [HttpGet]
        public async Task<IActionResult> MyDiary(int athleteId, DateTime? date, string? searchTerm = null, int pageIndex = 1, int pageSize = 2)
        {
            date ??= DateTime.UtcNow.Date; 
            var model = await this.foodDiaryService.GetModelByDateAndAthleteIdAsync(athleteId, date.Value);

            if (model != null && model.AthleteId != await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            model ??= await this.foodDiaryService.CreateForDateAsync(athleteId, date.Value);

            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            model.Foods = await this.foodService.GetPageModelsAsync(searchTerm, pageIndex, pageSize);
            this.ViewBag.SearchTerm = searchTerm;

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int diaryId)
        {
            var model = await this.foodDiaryService.GetDetailsByIdAsync(diaryId);

            if (model == null)
            {
                return this.NotFound();
            }

            if (model.AthleteId != await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFood(int foodId, int foodDiaryId, string mealType, int quantity)
        {
            var foodDiary = await this.foodDiaryService.AddFoodAsync(foodId, foodDiaryId, mealType, quantity);

            this.TempData["SuccessMessage"] = "Food successfully added!";
            return this.RedirectToAction("MyDiary", new { athleteId = foodDiary.AthleteId, date = foodDiary.Date, string.Empty, pageIndex = 1, pageSize = 2 });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFood(int diaryId, int foodId, string mealType)
        {
            await this.foodDiaryService.RemoveFoodAsync(diaryId, foodId, mealType);

            this.TempData["SuccessMessage"] = "Food successfully removed!";
            return this.RedirectToAction("Details", new { diaryId });
        }
    }
}
