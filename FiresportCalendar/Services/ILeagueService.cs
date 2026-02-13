using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ILeagueService
    {
        Task<League?> GetLeagueById(int leagueId);    
        Task<List<League>> GetAllLeagues();
    }
}
