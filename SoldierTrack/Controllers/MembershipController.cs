namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Membership;
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Controllers.Base;
    using SoldierTrack.Web.Models.Membership;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;
    using static SoldierTrack.Web.Common.Constants.WebConstants;

    public class MembershipController : BaseController
    {
        private readonly IMembershipService membershipService;
        private readonly IMapper mapper;

        public MembershipController(IMembershipService membershipService, IMapper mapper)
        {
            this.membershipService = membershipService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetArchive(int pageIndex = 1, int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.membershipService.GetArchiveByAthleteIdAsync(this.User.GetId()!, pageIndex, pageSize);
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RequestMembership()
        {
            var athleteId = this.User.GetId();

            if (await this.membershipService.MembershipExistsByAthleteIdAsync(athleteId!))
            {
                return this.RedirectToAction("Details", "Athlete", new { id = athleteId });
            }

            var viewModel = new MembershipFormModel()
            {
                AthleteId = athleteId!,
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

            var serviceModel = this.mapper.Map<MembershipServiceModel>(viewModel);
            await this.membershipService.RequestAsync(serviceModel);

            this.TempData["SuccessMessage"] = MembershipRequested;
            return this.RedirectToAction("Details", "Athlete", new { id = viewModel.AthleteId });
        }
    }
}
