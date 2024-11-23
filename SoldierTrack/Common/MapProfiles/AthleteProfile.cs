namespace SoldierTrack.Web.Common.MapProfiles
{
    using AutoMapper;
    using Models.Athlete;
    using Services.Athlete.Models;

    public class AthleteProfile : Profile
    {
        public AthleteProfile()
        {
            this.CreateMap<AthleteFormModel, AthleteServiceModel>()
                .ReverseMap();
        }
    }
}
