using FiresportCalendar.Models;
using System.Diagnostics;

namespace FiresportCalendar.Services
{
    public interface IRaceService
    {
        Task<List<Race>> GetAllAsync();
        Task<List<Race>> GetAllUpcomingRacesAsync();
        Task<Race?> GetByIdAsync(int raceId);
        Task<List<Race>> GetByIdsAsync(List<int> raceIds);
        Task<List<Race>> GetByTeamIdAsync(int teamId);    
        Task<List<Race>> GetTimerRacesAsync();
        Task UpdateAsync(Race race);
        Task DeleteByIdAsync(int id);
        Task AddAsync(Race race);

    }
}
