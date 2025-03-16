using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class RaceService : IRaceService
    {
        public readonly ApplicationDbContext _context;
        private readonly ITeamRaceService _teamRaceService;
        public RaceService( ApplicationDbContext context, ITeamRaceService teamRaceService) { 
            _context = context;
            _teamRaceService = teamRaceService;
           
        }
        public async Task<List<Race>> GetAllRaces()
        {
            return await _context.Races.Include(r => r.League).OrderBy(r => r.DateTime).ToListAsync();
        }
        public async Task<List<Race>> GetAllUpcomingRaces()
        {
            return await _context.Races.Include(r => r.League).Where(r => r.DateTime > DateTime.Today).OrderBy(r => r.DateTime).ToListAsync();
        }
        public async Task<Race?> GetRaceById(int raceId)
        {
            return await _context.Races.FindAsync(raceId);
        }
        public async Task<List<Race>> GetRacesByTeamId(int teamId)
        {
            var res = await _context.Races.Where(r => r.TeamRaces.Any(t => t.TeamId == teamId)).OrderBy(r => r.DateTime).ToListAsync();
            return res;
        }
        public async Task<List<Race>> GetTimerRaces()
        {
            var res = await _context.Races.Where(r => r.Timer && r.DateTime > DateTime.Today).OrderBy(r => r.DateTime).ToListAsync();
            return res;
        }
        public async Task UpdateRaceAsync(Race race)
        {
            race.TeamRaces = await _teamRaceService.GetAllTeamRacesByRaceId(race.Id);

            _context.Entry(race).State = EntityState.Modified;
            
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
        
        public async Task AddRaceAsync(Race race)
        {
            _context.Races.Add(race);
            await _context.SaveChangesAsync();

        }
    }
}
