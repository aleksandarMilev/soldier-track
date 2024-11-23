namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Base;
    using Common.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Membership;
    using Services.Membership;
    using Services.Membership.Models;

    using static Common.Constants.MessageConstants;
    using static Common.Constants.WebConstants;

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
        public async Task<IActionResult> GetArchive(int pageIndex = DefaultPageIndex, int pageSize = DefaultPageSize)
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
