namespace SoldierTrack.Services.Membership
{
    using SoldierTrack.Services.Membership.Models;

    public interface IMembershipService
    {
        Task RequestAsync(CreateMembershipServiceModel model);
    }
}
