namespace SoldierTrack.Services.Membership
{
    using Models;

    public interface IMembershipService
    {
        Task<MembershipPageServiceModel> GetArchiveByAthleteId(
            string athleteId,
            int pageIndex,
            int pageSize);

        Task<int> GetPendingCount();

        Task<bool> MembershipExistsByAthleteId(string athleteId);

        Task<bool> MembershipIsApprovedByAthleteId(string athleteId);

        Task<bool> MembershipIsExpiredByAthleteId (string athleteId);

        Task Request(MembershipServiceModel model);

        Task Approve(int id);

        Task Reject(int id);

        Task DeleteById(int id);

        Task DeleteByAthleteId(string athleteId);

        Task DeleteIfExpired(string athleteId);

        Task UpdateMembershipOnWorkoutDeletion(int? membershipId);

        Task UpdateMembershipOnJoinByAthleteId(string athleteId);

        Task UpdateMembershipOnLeaveByAthleteId(string athleteId);
    }
}
