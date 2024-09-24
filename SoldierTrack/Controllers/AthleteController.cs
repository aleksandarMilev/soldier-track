namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Web.Common.Attributes.Filter;
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
        [AthleteAuthorization]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.athleteService.GetDetailsModelByIdAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            if (!this.User.IsAdmin() && this.User.GetId() != model?.UserId)
            {
                return this.Unauthorized();
            }

            return this.View(model);
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

            if (await this.athleteService.AthleteWithSameNumberExistsAsync(serviceModel.PhoneNumber))
            {
                this.ModelState.AddModelError(nameof(serviceModel.PhoneNumber), string.Format(AthleteWithSameNumberExists, serviceModel.PhoneNumber));
                return this.View(viewModel);
            }

            await this.athleteService.CreateAsync(serviceModel);

            this.TempData["SuccessMessage"] = AthleteSuccessRegister;
            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AthleteAuthorization]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this.athleteService.GetEditServiceModelByIdAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            //we should map it because partial view requires class which is AthleteBaseFormModel or descendant 
            var viewModel = this.mapper.Map<EditAthleteViewModel>(serviceModel);

            if (!this.User.IsAdmin() && this.User.GetId() != viewModel.UserId)
            {
                return this.Unauthorized();
            }

            return this.View(viewModel);
        }

        [HttpPost]
        [AthleteAuthorization]
        public async Task<IActionResult> Edit(EditAthleteViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            if (!this.User.IsAdmin() && this.User.GetId() != viewModel.UserId)
            {
                return this.Unauthorized();
            }

            if (await this.athleteService.AthleteWithSameNumberExistsAsync(viewModel.PhoneNumber, viewModel.Id))
            {
                this.ModelState.AddModelError(nameof(viewModel.PhoneNumber), string.Format(AthleteWithSameNumberExists, viewModel.PhoneNumber));
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<EditAthleteServiceModel>(viewModel);
            await this.athleteService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = this.User.IsAdmin() ? AdminEditAthlete : AthleteEditHimself;
            return this.RedirectToAction(nameof(Details), new { id = serviceModel.Id });
        }

        [HttpPost]
        [AthleteAuthorization]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            if (!this.User.IsAdmin() && this.User.GetId() != userId)
            {
                return this.Unauthorized();
            }

            await this.athleteService.DeleteAsync(id);

            this.TempData["SuccessMessage"] = this.User.IsAdmin() ? AdminDeleteAthlete : AthleteDeleteHimself;
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }


        [HttpPost]
        [AthleteAuthorization]
        public async Task<IActionResult> Join(int athleteId, int workoutId)
        {
            await this.athleteService.JoinAsync(athleteId, workoutId);

            this.TempData["SuccessMessage"] = JoinSuccess;
            return this.RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AthleteAuthorization]
        public async Task<IActionResult> Leave(int athleteId, int workoutId)
        {
            await this.athleteService.LeaveAsync(athleteId, workoutId);

            this.TempData["SuccessMessage"] = LeaveSuccess;
            return this.RedirectToAction("Index", "Home");
        }
    }
}
