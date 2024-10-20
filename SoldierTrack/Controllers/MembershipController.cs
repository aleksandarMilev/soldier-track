namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Membership;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    [Authorize]
    public class MembershipController : Controller
    {
        private readonly IMembershipService membershipService;
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public MembershipController(
            IMembershipService membershipService,
            IAthleteService athleteService,
            IMapper mapper)
        {
            this.membershipService = membershipService;
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> RequestMembership(string athleteId)
        {
            if (await this.membershipService.MembershipExistsByAthleteIdAsync(athleteId))
            {
                return this.RedirectToAction("Details", "Athlete", new { id = athleteId });
            }

            var viewModel = new MembershipFormModel()
            {
                AthleteId = athleteId,
                StartDate = DateTime.Now
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RequestMembership(MembershipFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            viewModel.StartDate = DateTime.UtcNow;
            var serviceModel = this.mapper.Map<MembershipServiceModel>(viewModel);
            await this.membershipService.RequestAsync(serviceModel);

            this.TempData["SuccessMessage"] = MembershipRequested;
            return this.RedirectToAction("Details", "Athlete", new { id = viewModel.AthleteId });
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(string athleteId, int pageIndex = 1, int pageSize = 5)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            if (this.User.GetId() != athleteId)
            {
                return this.Unauthorized();
            }

            this.ViewBag.AthleteId = athleteId;
            var model = await this.membershipService.GetArchiveByAthleteIdAsync(athleteId, pageIndex, pageSize);
            return this.View(model);
        }
    }
}
