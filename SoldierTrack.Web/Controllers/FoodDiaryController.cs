namespace SoldierTrack.Web.Controllers
{
    using Base;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.Athlete;
    using Services.FoodDiary;

    using static Constants.MessageConstants;

    public class FoodDiaryController : BaseController
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

            this.ValidateDate(date);

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

        private DateTime? ValidateDate(DateTime? date)
        {
            if ((date != null) && (DateTime.Now < date.Value.AddMonths(-1) || DateTime.Now > date.Value.AddMonths(1)))
            {
                this.TempData["FailureMessage"] = string.Format(
                    FoodDiaryDateError,
                    DateTime.Now.AddMonths(-1).ToString("dd MMM yyyy"),
                    DateTime.Now.AddMonths(1).ToString("dd MMM yyyy"));

                return DateTime.Now;
            }

            return date;
        }
    }
}
