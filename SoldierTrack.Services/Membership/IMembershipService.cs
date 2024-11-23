namespace SoldierTrack.Services.Membership
{
    using Models;

    public interface IMembershipService
    {
        Task<MembershipPageServiceModel> GetArchiveByAthleteIdAsync(string athleteId, int pageIndex, int pageSize);

        Task<int> GetPendingCountAsync();

        Task<bool> MembershipExistsByAthleteIdAsync(string athleteId);

        Task<bool> MembershipIsApprovedByAthleteIdAsync(string athleteId);

        Task<bool> MembershipIsExpiredByAthleteIdAsync (string athleteId);

        Task RequestAsync(MembershipServiceModel model);

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
