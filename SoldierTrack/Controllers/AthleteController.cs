namespace SoldierTrack.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SoldierTrack.Data.Models;
    using SoldierTrack.Services.Athlete;
    using SoldierTrack.Services.Athlete.Models;
    using SoldierTrack.Web.Common.Extensions;
    using SoldierTrack.Web.Models.Athlete;

    using static SoldierTrack.Web.Common.Constants.MessageConstants;

    [Authorize]
    public class AthleteController : Controller
    {
        private readonly IAthleteService athleteService;
        private readonly SignInManager<Athlete> signInManager;
        private readonly IMapper mapper;

        public AthleteController(
            IAthleteService athleteService,
            SignInManager<Athlete> signInManager,
            IMapper mapper)
        {
            this.athleteService = athleteService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var model = await this.athleteService.GetDetailsModelByIdAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            if (!this.User.IsAdmin() && this.User.GetId() != model.Id)
            {
                return this.Unauthorized();
            }

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var serviceModel = await this.athleteService.GetFormModelByIdAsync(id);

            if (serviceModel == null)
            {
                return this.NotFound();
            }

            var viewModel = this.mapper.Map<AthleteFormModel>(serviceModel);

            if (!this.User.IsAdmin() && this.User.GetId() != viewModel.Id)
            {
                return this.Unauthorized();
            }

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AthleteFormModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(viewModel);
            }

            if (!this.User.IsAdmin() && this.User.GetId() != viewModel.Id)
            {
                return this.Unauthorized();
            }

            if (await this.athleteService.AthleteWithSameNumberExistsAsync(viewModel.PhoneNumber, viewModel.Id))
            {
                this.ModelState.AddModelError(nameof(viewModel.PhoneNumber), string.Format(PhoneDuplicate, viewModel.PhoneNumber));
                return this.View(viewModel);
            }

            if (await this.athleteService.AthleteWithSameEmailExistsAsync(viewModel.Email, viewModel.Id))
            {
                this.ModelState.AddModelError(nameof(viewModel.Email), string.Format(EmailDuplicate, viewModel.Email));
                return this.View(viewModel);
            }

            var serviceModel = this.mapper.Map<AthleteServiceModel>(viewModel);
            await this.athleteService.EditAsync(serviceModel);

            this.TempData["SuccessMessage"] = this.User.IsAdmin() ? AdminEditAthlete : AthleteEditHimself;
            return this.RedirectToAction(nameof(Details), new { id = serviceModel.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!this.User.IsAdmin() && this.User.GetId() != id)
            {
                return this.Unauthorized();
            }

            if (!this.User.IsAdmin())
            {
                await this.signInManager.SignOutAsync();
            }

            await this.athleteService.DeleteAsync(id);

            this.TempData["SuccessMessage"] = this.User.IsAdmin() ? AdminDeleteAthlete : AthleteDeleteHimself;
            return this.RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Join(string athleteId, int workoutId)
        {
            await this.athleteService.JoinAsync(this.User.GetId()!, workoutId);

            this.TempData["SuccessMessage"] = JoinSuccess;
            return this.RedirectToAction("Details", "Workout", new { id = workoutId });
        }

        [HttpPost]
        public async Task<IActionResult> Leave(string athleteId, int workoutId)
        {
            await this.athleteService.LeaveAsync(athleteId, workoutId);

            this.TempData["SuccessMessage"] = this.User.IsAdmin() ? AdminLeaveSuccess : AthleteLeaveSuccess;
            return this.RedirectToAction("Details", "Workout", new { id = workoutId });
        }
    }
}
