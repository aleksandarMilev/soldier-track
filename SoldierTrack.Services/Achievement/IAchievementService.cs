namespace SoldierTrack.Services.Achievement
{
    using Common;
    using Exercise.Models.Util;
    using Models;

    /// <summary>
    /// Provides querying and management operations for athlete achievements,
    /// including pagination, lookups, rankings, and CRUD.
    /// </summary>
    public interface IAchievementService
    {
        /// <summary>
        /// Retrieves a paged list of achievements for a specific athlete.
        /// </summary>
        /// <param name="athleteId">The athlete's identifier.</param>
        /// <param name="pageIndex">1-based page index.</param>
        /// <param name="pageSize">Page size (number of records per page).</param>
        /// <returns>
        /// A page result that contains the items and pagination metadata.
        /// </returns>
        /// <remarks>
        /// Results are ordered by the related exercise name ascending.
        /// </remarks>
        Task<PaginatedModel<AchievementServiceModel>> GetAllByAthleteId(
            string athleteId,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// Gets the achievement id for an athlete-exercise pair.
        /// </summary>
        /// <param name="athleteId">The athlete's identifier.</param>
        /// <param name="exerciseId">The exercise identifier.</param>
        /// <returns>
        /// The achievement id if present; otherwise, <c>null</c>.
        /// </returns>
        Task<int?> GetAchievementId(
            string athleteId,
            int exerciseId);

        /// <summary>
        /// Gets the top rankings (by 1RM) for a given exercise.
        /// </summary>
        /// <param name="exerciseId">The exercise identifier.</param>
        /// <returns>
        /// A collection of up to 10 ranked entries ordered by one-rep max descending.
        /// </returns>
        Task<IEnumerable<Ranking>> GetRankings(int exerciseId);   
        
        /// <summary>
        /// Gets a single achievement by its identifier.
        /// </summary>
        /// <param name="id">Achievement identifier.</param>
        /// <returns>The achievement if found; otherwise, <c>null</c>.</returns>
        Task<AchievementServiceModel?> GetById(int id);

        /// <summary>
        /// Checks whether an athlete already has an achievement for the given exercise.
        /// </summary>
        /// <param name="exerciseId">The exercise identifier.</param>
        /// <param name="athleteId">The athlete's identifier.</param>
        /// <returns><c>true</c> if one exists; otherwise, <c>false</c>.</returns>
        Task<bool> AchievementIsAlreadyAdded(
            int exerciseId,
            string athleteId);

        /// <summary>
        /// Creates a new achievement.
        /// </summary>
        /// <param name="model">The achievement to create.</param>
        /// <param name="athleteId">The athlete's identifier performing the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Create(
            AchievementServiceModel model,
            string athleteId);

        /// <summary>
        /// Updates an existing achievement.
        /// </summary>
        /// <param name="model">The updated achievement model.</param>
        /// <param name="athleteId">The athlete's identifier performing the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the achievement does not exist or the operation is unauthorized.</exception>
        Task Edit(
            AchievementServiceModel model,
            string athleteId);

        /// <summary>
        /// Deletes an achievement if it belongs to the specified athlete.
        /// </summary>
        /// <param name="achievementId">The achievement identifier.</param>
        /// <param name="athleteId">The athlete's identifier performing the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the achievement does not exist or the operation is unauthorized.
        /// </exception>
        Task Delete(
            int achievementId,
            string athleteId);
    }
}
