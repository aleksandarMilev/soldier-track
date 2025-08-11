namespace SoldierTrack.Web.Controllers
{
    using Base;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Services.FoodDiary;

    using static Constants.MessageConstants;

    public class FoodDiaryController(
        IFoodDiaryService service) : BaseController
    {
        private readonly IFoodDiaryService service = service;

        [HttpGet]
        public async Task<IActionResult> MyDiary(DateTime? date)
        {
            var athleteId = this.User.GetId();

            this.ValidateDate(date);

            date ??= DateTime.Now; 

            var model = await this.service.GetModelByDateAndAthleteId(
                athleteId!,
                date.Value.Date);

            model ??= await this.service.CreateForDate(
                athleteId!,
                date.Value);

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int diaryId)
        {
            var model = await this.service.GetDetailsById(diaryId);

            if (model is null)
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
        public async Task<IActionResult> AddFood(
            int foodId,
            DateTime date,
            string mealType,
            int quantity)
        {
            await this.service.AddFoodAsync(
                this.User.GetId()!,
                foodId,
                date.Date,
                mealType,
                quantity);

            this.TempData["SuccessMessage"] = "Food successfully added!";

            return this.RedirectToAction("GetAll", "Food");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFood(
            int diaryId,
            int foodId,
            string mealType)
        {
            await this.service.RemoveFoodAsync(diaryId, foodId, mealType);

            this.TempData["SuccessMessage"] = "Food successfully removed!";

            return this.RedirectToAction(
                nameof(this.Details),
                new { diaryId });
        }

        private DateTime? ValidateDate(DateTime? date)
        {
            var dateIsNotNullAndIsInTheCorrectRange = 
                (date != null) &&
                (DateTime.Now < date.Value.AddMonths(-1) ||
                DateTime.Now > date.Value.AddMonths(1));

            if (dateIsNotNullAndIsInTheCorrectRange)
            {
                const string DateFormat = "dd MMM yyyy";

                this.TempData["FailureMessage"] = string.Format(
                    FoodDiaryDateError,
                    DateTime.Now.AddMonths(-1).ToString(DateFormat),
                    DateTime.Now.AddMonths(1).ToString(DateFormat));

                return DateTime.Now;
            }

            return date;
        }
    }
}
