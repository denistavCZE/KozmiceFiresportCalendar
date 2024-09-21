using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

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
