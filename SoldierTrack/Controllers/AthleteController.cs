namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Athlete.Models.Base;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Athlete;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [Authorize]
    public class AthleteController : Controller
    {
        private readonly IAthleteService athleteService;
        private readonly IMapper mapper;

        public AthleteController(IAthleteService athleteService, IMapper mapper)
        {
            this.athleteService = athleteService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateAthleteViewModel();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAthleteViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            viewModel.UserId = this.User.GetId();
            var serviceModel = this.mapper.Map<AthleteServiceModel>(viewModel);

            if (await this.athleteService.IsAthleteWithSameNumberExistsAsync(serviceModel.PhoneNumber))
            {
                this.ModelState.AddModelError(nameof(serviceModel.PhoneNumber), string.Format(AthleteWithSameNumberExists, serviceModel.PhoneNumber));
                return this.View(viewModel);
            }

            await this.athleteService.CreateAsync(serviceModel);
            this.TempData["SuccessMessage"] = AthleteSuccessRegister;
            return this.RedirectToAction("Index", "Home");
        }
    }
}
