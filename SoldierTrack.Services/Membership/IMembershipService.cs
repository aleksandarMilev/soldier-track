namespace SoldierTrack.Services.Membership
{
    using SoldierTrack.Services.Membership.Models;

    public interface IMembershipService
    {
        Task RequestAsync(CreateMembershipServiceModel model);

        Task<IEnumerable<MembershipPendingServiceModel>> GetAllPendingAsync();

        Task<int> GetPendingCountAsync();

        Task ApproveAsync(int id);

        Task RejectAsync(int id);
    }
}
