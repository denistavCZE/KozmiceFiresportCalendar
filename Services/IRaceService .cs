using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface IRaceService
    {
        Task<List<Race>> GetAllRaces();
        Task<List<Race>> GetAllUpcomingRaces();

        Task<Race?> GetRaceById(int raceId);
        Task<List<Race>> GetRacesByTeamId(int teamId);    
        Task<List<Race>> GetTimerRaces();

    }
}
