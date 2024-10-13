namespace SoldierTrack.Web.Controllers
{
    using Humanizer.DateTimeHumanizeStrategy;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.FoodDiary;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;

    [AthleteAuthorization]
    public class FoodDiaryController : Controller
    {
        private readonly IFoodDiaryService foodDiaryService;
        private readonly IAthleteService athleteService;

        public FoodDiaryController(
            IFoodDiaryService foodDiaryService,
            IAthleteService athleteService)
        {
            this.foodDiaryService = foodDiaryService;
            this.athleteService = athleteService;
        }

        [HttpGet]
        public async Task<IActionResult> MyDiary(int athleteId, DateTime? date)
        {
            date ??= DateTime.UtcNow.Date; 
            var model = await this.foodDiaryService.GetModelByDateAndAthleteIdAsync(athleteId, date.Value);

            if (model != null && model.AthleteId != await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            model ??= await this.foodDiaryService.CreateForDateAsync(athleteId, date.Value);
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
        public async Task<IActionResult> AddFood(int athleteId, int foodId, DateTime date, string mealType, int quantity)
        {
            if (athleteId != await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!))
            {
                return this.Unauthorized();
            }

            var foodDiary = await this.foodDiaryService.AddFoodAsync(athleteId, foodId, date, mealType, quantity);

            this.TempData["SuccessMessage"] = "Food successfully added!";
            return this.RedirectToAction("GetAll", "Food");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFood(int diaryId, int foodId, string mealType)
        {
            await this.foodDiaryService.RemoveFoodAsync(diaryId, foodId, mealType);

            this.TempData["SuccessMessage"] = "Food successfully removed!";
            return this.RedirectToAction(nameof(Details), new { diaryId });
        }
    }
}
