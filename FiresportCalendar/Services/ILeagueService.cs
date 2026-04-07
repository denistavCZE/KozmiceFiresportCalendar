using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ILeagueService
    {
        Task<League?> GetByIdAsync(int leagueId);    
        Task<List<League>> GetAllAsync();
        Task UpdateAsync(League model);
        Task DeleteByIdAsync(int id);
        Task AddAsync(League league);

    }
}
