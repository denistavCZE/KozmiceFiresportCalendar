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
        public RaceService( ApplicationDbContext context) { 
            _context = context;
           
        }
        public async Task<List<Race>> GetAllRaces()
        {
            return await _context.Races.ToListAsync();
        }
        public async Task<List<Race>> GetAllUpcomingRaces()
        {
            return await _context.Races.Where(r => r.DateTime > DateTime.Today).ToListAsync();
        }
        public async Task<Race?> GetRaceById(int raceId)
        {
            return await _context.Races.FirstAsync(r => r.Id == raceId);
        }
        public async Task<List<Race>> GetRacesByTeamId(int teamId)
        {
            var res = await _context.Races.Where(r => r.TeamRaces.Any(t => t.TeamId == teamId)).OrderBy(r => r.DateTime).ToListAsync();
            return res;
        }
        public async Task<List<Race>> GetTimerRaces()
        {
            var res = await _context.Races.Where(r => r.Timer).OrderBy(r => r.DateTime).ToListAsync();
            return res;
        }

    }
}
