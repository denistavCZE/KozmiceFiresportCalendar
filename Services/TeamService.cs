using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class TeamService : ITeamService
    {
        public readonly ApplicationDbContext _context;
        public TeamService( ApplicationDbContext context) { 
            _context = context;
           
        }
        public async Task<List<Team>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
 
        }
    }
}
