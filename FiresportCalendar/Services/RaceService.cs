using FiresportCalendar.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class RaceService : IRaceService
    {
        public readonly ApplicationDbContext _context;
        public RaceService( ApplicationDbContext context) { 
            _context = context;           
        }
        public async Task<List<Race>> GetAllAsync()
        {
            return await _context.Races.Include(r => r.League).OrderBy(r => r.DateTime).ToListAsync();
        }
        public async Task<List<Race>> GetAllUpcomingRacesAsync()
        {
            return await _context.Races.Include(r => r.League).Where(r => r.DateTime > DateTime.Today).OrderBy(r => r.DateTime).ToListAsync();
        }
        public async Task<Race?> GetByIdAsync(int raceId)
        {
            return await _context.Races.FindAsync(raceId);
        }
        public async Task<List<Race>> GetByIdsAsync(List<int> raceIds)
        {
            return await _context.Races.Include(r => r.League).Where(r => raceIds.Contains(r.Id)).OrderBy(r => r.DateTime).ToListAsync();
        }
        public async Task<List<Race>> GetByTeamIdAsync(int teamId)
        {
            var res = await _context.Races.Where(r => r.TeamRaces.Any(t => t.TeamId == teamId)).OrderBy(r => r.DateTime).ToListAsync();
            return res;
        }
        public async Task<List<Race>> GetTimerRacesAsync()
        {
            var res = await _context.Races.Where(r => r.Timer && r.DateTime > DateTime.Today).OrderBy(r => r.DateTime).ToListAsync();
            return res;
        }
        public async Task UpdateAsync(Race model)
        {
            var race = await _context.Races.FindAsync(model.Id);

            if (race == null)
                throw new Exception("Závod nenalezen.");

            race.Place = model.Place;
            race.DateTime = model.DateTime;
            race.Timer = model.Timer;
            race.LeagueId = model.LeagueId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var race = await _context.Races.FindAsync(id);
            if (race != null)
            {
                _context.Races.Remove(race);
            }

            await _context.SaveChangesAsync();
        }
        
        public async Task AddAsync(Race race)
        {
            _context.Races.Add(race);
            await _context.SaveChangesAsync();

        }
    }
}
