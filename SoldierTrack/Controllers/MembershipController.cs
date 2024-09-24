namespace SoldierTrack.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Common.CustomMapping;
    using SoldierTrack.Web.Models.Membership;

    using static SoldierTrack.Web.Common.Constants.WebConstants;
    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [AthleteAuthorization]
    public class MembershipController : Controller
    {
        private readonly IMembershipService membershipService;
        private readonly IAthleteService athleteService;

        public MembershipController(
            IMembershipService membershipService,
            IAthleteService athleteService)
        {
            this.membershipService = membershipService;
            this.athleteService = athleteService;
        }

        [HttpGet]
        public async Task<IActionResult> RequestMembership()
        {
            var userId = this.User.GetId();
            var athleteId = await this.athleteService.GetIdByUserIdAsync(userId!);

            if (await this.athleteService.AthleteHasMembershipAsync(athleteId))
            {
                return this.RedirectToAction("Details", "Athlete", new { id = athleteId });
            }

            var viewModel = new CreateMembershipViewModel()
            {
                AthleteId = athleteId,
                StartDate = DateTime.Now.Date
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RequestMembership(CreateMembershipViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            var serviceModel = viewModel.MapToCreateMembershipServiceModel();
            await this.membershipService.RequestAsync(serviceModel);

            this.TempData["SuccessMessage"] = MembershipRequested;
            return this.RedirectToAction("Details", "Athlete", new { id = viewModel.AthleteId });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(int athleteId, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            this.ViewBag.AthleteId = athleteId;
            var model = await this.membershipService.GetArchiveByAthleteIdAsync(athleteId, pageIndex, pageSize);
            return this.View(model);
        }
    }
}
