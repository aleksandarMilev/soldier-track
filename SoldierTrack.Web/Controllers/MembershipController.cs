namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Base;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models.Membership;
    using Services.Membership;
    using Services.Membership.Models;

    using static Constants.MessageConstants;
    using static Constants.WebConstants;

    public class MembershipController(
        IMembershipService service,
        IMapper mapper) : BaseController
    {
        private readonly IMembershipService service = service;
        private readonly IMapper mapper = mapper;

        [HttpGet]
        public async Task<IActionResult> GetArchive(
            int pageIndex = DefaultPageIndex,
            int pageSize = DefaultPageSize)
        {
            pageSize = Math.Min(pageSize, MaxPageSize);
            pageSize = Math.Max(pageSize, MinPageSize);

            var model = await this.service.GetArchiveByAthleteId(
                this.User.GetId()!,
                pageIndex,
                pageSize);

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RequestMembership()
        {
            var athleteId = this.User.GetId();

            if (await this.service.MembershipExistsByAthleteId(athleteId!))
            {
                return this.RedirectToAction(
                    "Details",
                    "Athlete",
                    new { id = athleteId });
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
            await this.service.Request(serviceModel);

            this.TempData["SuccessMessage"] = MembershipRequested;

            return this.RedirectToAction(
                "Details",
                "Athlete",
                new { id = viewModel.AthleteId });
        }
    }
}
