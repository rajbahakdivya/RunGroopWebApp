using RunGroopWebApp.Models;

namespace RunGroopWebApp.Interfaces
{
    public interface IRaceRepository
    {
        Task<IEnumerable<Race>> GetAll();
        Task<Race> GetByIdAsync(int id);
        Task<Race> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Race>> GetAllRacesByCity(string city);

        // Return Task<bool> to allow async operations
        Task<bool> Add(Race race);
        Task<bool> Update(Race race);
        Task<bool> Delete(Race race);

        // Return Task<bool> for async save operation
        Task<bool> Save();
    }
}
