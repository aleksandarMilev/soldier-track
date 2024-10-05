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

        public async Task<IActionResult> MyDiary(int athleteId, DateTime? date, string? searchTerm = null, int pageIndex = 1, int pageSize = 3)
        {
            date ??= DateTime.UtcNow.Date; 
            var model = await this.foodDiaryService.GetByDateAndAthleteIdAsync(athleteId, date.Value);
            var currentAthleteId = await this.athleteService.GetIdByUserIdAsync(this.User.GetId()!);

            if (model != null && model.AthleteId != currentAthleteId)
            {
                return this.Unauthorized();
            }

            model ??= await this.foodDiaryService.CreateForDateAsync(athleteId, date.Value);

            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            model.FoodEntries = await this.foodService.GetPageModelsAsync(searchTerm, pageIndex, pageSize);
            this.ViewBag.SearchTerm = searchTerm;

            return this.View(model);

        }
    }
}
