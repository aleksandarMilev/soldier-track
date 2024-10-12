namespace SoldierTrack.Services.Membership
{
    using SoldierTrack.Services.Membership.Models;
    using SoldierTrack.Services.Membership.Models.Base;

    public interface IMembershipService
    {
        Task RequestAsync(CreateMembershipServiceModel model);

        Task<IEnumerable<MembershipPendingServiceModel>> GetAllPendingAsync();

        Task<MembershipArchivePageServiceModel> GetArchiveByAthleteIdAsync(int athleteId, int pageIndex, int pageSize);

        Task<bool> MembershipExistsByAthleteIdAsync(int athleteId);

        Task<bool> MembershipIsApprovedByAthleteIdAsync(int athleteId);

        Task<bool> MembershipIsExpiredByAthleteIdAsync (int athleteId);

        Task<int> GetPendingCountAsync();

        Task ApproveAsync(int id);

        Task RejectAsync(int id);

        Task DeleteByIdAsync(int id);

        Task DeleteByAthleteIdAsync(int athleteId);

        Task UpdateMembershipOnWorkoutDeletionAsync(int? membershipId);

        Task UpdateMembershipOnJoinByAthleteIdAsync(int athleteId);

        Task UpdateMembershipOnLeaveByAthleteIdAsync(int athleteId);
    }
}
