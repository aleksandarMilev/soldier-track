namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using SoldierTrack.Services.Athlete.Models.Base;
    using SoldierTrack.Web.Models.Athlete;

    public class AthleteProfile : Profile
    {
        public AthleteProfile()
        {
            this.CreateMap<CreateAthleteViewModel, AthleteServiceModel>();
        }
    }
}
