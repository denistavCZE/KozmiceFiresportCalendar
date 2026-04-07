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
        public async Task<League?> GetByIdAsync(int leagueId)
        {
            return await _context.Leagues.FirstOrDefaultAsync(l => l.Id == leagueId);
        }
        public async Task<List<League>> GetAllAsync()
        {
            return await _context.Leagues.ToListAsync();
        }

        public async Task UpdateAsync(League model)
        {
            var league = await _context.Leagues.FindAsync(model.Id);

            if (league == null)
                throw new Exception("Liga nenalezena.");

            league.Name = model.Name;
            league.Color= model.Color;
           
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var league = await _context.Leagues.FindAsync(id);
            if (league != null)
            {
                _context.Leagues.Remove(league);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(League league)
        {
            _context.Leagues.Add(league);
            await _context.SaveChangesAsync();

        }

    }
}
