using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface IRaceService
    {
        Task<Race?> GetRaceById(int raceId);
        Task<List<Race>> GetRacesByTeamId(int teamId);    
        Task<List<Race>> GetTimerRaces();
        Task<League?> GetLeague(int leagueId);

    }
}
