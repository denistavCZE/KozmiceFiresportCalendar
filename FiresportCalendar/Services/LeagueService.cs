using FiresportCalendar.Models;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class LeagueService : ILeagueService
    {
        public readonly ApplicationDbContext _context;
        public LeagueService( ApplicationDbContext context) { 
            _context = context;
           
        }
        public async Task<League?> GetLeagueById(int leagueId)
        {
            return await _context.Leagues.FirstAsync(l => l.Id == leagueId);
        }
        public async Task<List<League>> GetAllLeagues()
        {
            return await _context.Leagues.ToListAsync();
        }


    }
}
