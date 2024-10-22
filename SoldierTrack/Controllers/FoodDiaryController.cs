namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.FoodDiary;
    using SoldierTrack.Web.Common.Extensions;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [Authorize]
    public class FoodDiaryController : Controller
    {
        private readonly IFoodDiaryService foodDiaryService;
        private readonly IAthleteService athleteService;

        public FoodDiaryController(IFoodDiaryService foodDiaryService, IAthleteService athleteService)
        {
            this.foodDiaryService = foodDiaryService;
            this.athleteService = athleteService;
        }

        [HttpGet]
        public async Task<IActionResult> MyDiary(DateTime? date)
        {
            var athleteId = this.User.GetId();

            if ((date != null) && (DateTime.Now < date.Value.AddMonths(-1) || DateTime.Now > date.Value.AddMonths(1)))
            {
                this.TempData["FailureMessage"] = string.Format(
                    FoodDiaryDateError,
                    DateTime.Now.AddMonths(-1).ToString("dd MMM yyyy"),
                    DateTime.Now.AddMonths(1).ToString("dd MMM yyyy"));

                date = DateTime.Now;
            }

            date ??= DateTime.Now; 

            var model = await this.foodDiaryService.GetModelByDateAndAthleteIdAsync(athleteId!, date.Value.Date);
            model ??= await this.foodDiaryService.CreateForDateAsync(athleteId!, date.Value);

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

            if (model.AthleteId != this.User.GetId())
            {
                return this.Unauthorized();
            }

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFood(int foodId, DateTime date, string mealType, int quantity)
        {
            await this.foodDiaryService.AddFoodAsync(this.User.GetId()!, foodId, date.Date, mealType, quantity);

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
