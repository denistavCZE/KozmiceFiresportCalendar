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
        public async Task<Team?> GetTeamById(int teamId)
        {
            return await _context.Teams.Include(t => t.People)
                                       .Include(t => t.Leagues)
                                       .Include(t => t.TeamRaces.Where(tr => tr.Race.DateTime > DateTime.Today))
                                            .ThenInclude(tr => tr.Race)
                                                .ThenInclude(r => r.League)
                                       .Include(t => t.TeamRaces.Where(tr => tr.Race.DateTime > DateTime.Today))
                                            .ThenInclude(tr => tr.TeamRacePeople)
                                       .FirstOrDefaultAsync(t => t.Id == teamId);
        }
        public async Task<List<Team>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
 
        }
        public async Task<List<Team>> GetActiveTeams()
        {
            return await _context.Teams.Where(t => t.Active).ToListAsync();
        }

        public async Task<bool> AddMember(int teamId, string personId)
        {
            var team = await _context.Teams.Include(t => t.People).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return false;

            var person = await _context.Users.FirstOrDefaultAsync(p => p.Id == personId);
            if(person == null)
                return false;

            if (team.People.Contains(person))
                return false;
            team.People.Add(person);

            _context.SaveChanges();

            return true;

        }

        public async Task<bool> RemoveMember(int teamId, string personId)
        {
            var team = await _context.Teams.Include(t => t.People).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return false;

            var person = await _context.Users.FirstOrDefaultAsync(p => p.Id == personId);
            if (person == null)
                return false;

            if (!team.People.Contains(person))
                return false;
            team.People.Remove(person);

            _context.SaveChanges();

            return true;

        }


        public async Task<bool> AddLeague(int teamId, int leagueId)
        {
            var team = await _context.Teams.Include(t => t.Leagues).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return false;

            var league = await _context.Leagues.FirstOrDefaultAsync(l => l.Id == leagueId);
            if (league == null)
                return false;

            if (team.Leagues.Contains(league))
                return false;
            team.Leagues.Add(league);

            _context.SaveChanges();

            return true;

        }
        public async Task<bool> RemoveLeague(int teamId, int leagueId)
        {
            var team = await _context.Teams.Include(t => t.Leagues).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return false;

            var league = await _context.Leagues.FirstOrDefaultAsync(l => l.Id == leagueId);
            if (league == null)
                return false;

            if (!team.Leagues.Contains(league))
                return false;
            team.Leagues.Remove(league);

            _context.SaveChanges();

            return true;

        }

        public async Task<List<League>> GetTeamLeagues(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return new List<League>();

            return team.Leagues.ToList();
        }
    }
}
