using FiresportCalendar.Models;
using System.Diagnostics;

namespace FiresportCalendar.Services
{
    public interface IRaceService
    {
        Task<List<Race>> GetAllRaces();
        Task<List<Race>> GetAllUpcomingRaces();
        Task<Race?> GetRaceById(int raceId);
        Task<List<Race>> GetRacesByIds(List<int> raceIds);
        Task<List<Race>> GetRacesByTeamId(int teamId);    
        Task<List<Race>> GetTimerRaces();
        Task UpdateRaceAsync(Race race);
        Task DeleteByIdAsync(int id);
        Task AddRaceAsync(Race race);

    }
}
