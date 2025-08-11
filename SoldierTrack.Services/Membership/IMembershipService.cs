namespace SoldierTrack.Services.Membership
{
    using Models;

    public interface IMembershipService
    {
        Task<MembershipPageServiceModel> GetArchiveByAthleteId(string athleteId, int pageIndex, int pageSize);

        Task<int> GetPendingCountAsync();

        Task<bool> MembershipExistsByAthleteId(string athleteId);

        Task<bool> MembershipIsApprovedByAthleteIdAsync(string athleteId);

        Task<bool> MembershipIsExpiredByAthleteIdAsync (string athleteId);

        Task Request(MembershipServiceModel model);

        Task Approve(int id);

        Task Reject(int id);

        Task DeleteById(int id);

        Task DeleteByAthleteIdAsync(string athleteId);

        Task DeleteIfExpired(string athleteId);

        Task UpdateMembershipOnWorkoutDeletionAsync(int? membershipId);

        Task UpdateMembershipOnJoinByAthleteIdAsync(string athleteId);

        Task UpdateMembershipOnLeaveByAthleteIdAsync(string athleteId);
    }
}
