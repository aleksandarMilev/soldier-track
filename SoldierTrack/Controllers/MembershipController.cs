namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Web.Common.Attributes.Filter;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Membership;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [AthleteAuthorization]
    public class MembershipController : Controller
    {
        private readonly IMapper mapper;
        private readonly IMembershipService membershipService;
        private readonly IAthleteService athleteService;

        public MembershipController(
            IMapper mapper,
            IMembershipService membershipService,
            IAthleteService athleteService)
        {
            this.mapper = mapper;
            this.membershipService = membershipService;
            this.athleteService = athleteService;
        }

        [HttpGet]
        public async Task<IActionResult> RequestMembership()
        {
            var userId = this.User.GetId();
            var athleteId = await this.athleteService.GetIdByUserIdAsync(userId!);

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

            var serviceModel = new CreateMembershipServiceModel();
            this.mapper.Map(viewModel, serviceModel);

            await this.membershipService.RequestAsync(serviceModel);

            this.TempData["SuccessMessage"] = MembershipRequested;
            return this.RedirectToAction("Index", "Home");
        }
    }
}
