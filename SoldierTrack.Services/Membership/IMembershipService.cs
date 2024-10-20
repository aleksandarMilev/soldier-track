namespace SoldierTrack.Services.Membership
{
    using SoldierTrack.Services.Membership.Models;

    public interface IMembershipService
    {
        Task RequestAsync(MembershipServiceModel model);

        Task<MembershipPageServiceModel> GetArchiveByAthleteIdAsync(string athleteId, int pageIndex, int pageSize);

        Task<bool> MembershipExistsByAthleteIdAsync(string athleteId);

        Task<bool> MembershipIsApprovedByAthleteIdAsync(string athleteId);

        Task<bool> MembershipIsExpiredByAthleteIdAsync (string athleteId);

        Task<int> GetPendingCountAsync();

        Task ApproveAsync(int id);

        Task RejectAsync(int id);

        Task DeleteByIdAsync(int id);

        Task DeleteByAthleteIdAsync(string athleteId);

        Task DeleteIfExpiredAsync(string athleteId);

        Task UpdateMembershipOnWorkoutDeletionAsync(int? membershipId);

        Task UpdateMembershipOnJoinByAthleteIdAsync(string athleteId);

        Task UpdateMembershipOnLeaveByAthleteIdAsync(string athleteId);
    }
}
