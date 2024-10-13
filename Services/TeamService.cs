using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;
using Microsoft.AspNetCore.Mvc;

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
                                                .ThenInclude(trp => trp.Person)
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

            await _context.SaveChangesAsync();

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

            await _context.SaveChangesAsync();

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

            await _context.SaveChangesAsync();

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

            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<bool> AddRace(int teamId, int raceId)
        {
            var team = await _context.Teams.Include(t => t.TeamRaces).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return false;

            var race = await _context.Races.FirstOrDefaultAsync(r => r.Id == raceId);
            if (race == null)
                return false;

            if (team.TeamRaces.Exists(tr => tr.RaceId == raceId))
                return false;

            TeamRace teamRace = new TeamRace(teamId, raceId);
            team.TeamRaces.Add(teamRace);

            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<bool> RemoveRace(int teamId, int raceId)
        {
            var team = await _context.Teams.Include(t => t.TeamRaces).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return false;
            
            var teamRace = await _context.TeamRaces.FirstOrDefaultAsync(tr => tr.RaceId == raceId && tr.TeamId == teamId);
            if (teamRace == null)    
                return false;

            team.TeamRaces.Remove(teamRace);

            await _context.SaveChangesAsync();

            return true;

        }
        public async Task<List<Person>> GetMembers(int teamId)
        {
            return (await _context.Teams.Include(t => t.People).FirstOrDefaultAsync(t => t.Id == teamId))?.People.ToList() ?? new List<Person>();
        }
        public async Task<bool> IsMember(int teamId, string personId)
        {
            var userName = (await _context.Users.FirstOrDefaultAsync(u => u.Id == personId))?.UserName;
            if (userName == null)
                return false;
            return (await GetMembers(teamId)).Any(m => m.UserName == userName);
        }
        public async Task<List<League>> GetTeamLeagues(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                return new List<League>();

            return team.Leagues.ToList();
        }
        
        public async Task AddUpcomingLeagueRaces(int teamId, int leagueId)
        {
            var upcomingLeagueRaces = await _context.Races.Where(r => r.DateTime > DateTime.Today && r.LeagueId == leagueId).ToListAsync();

            var team = await _context.Teams
                                .Include(t => t.TeamRaces)
                                    .ThenInclude(tr => tr.Race)
                                .FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null)
                throw new ArgumentException();

            foreach (var race in upcomingLeagueRaces)
                if (!team.TeamRaces.Any(tr => tr.RaceId == race.Id))
                    team.TeamRaces.Add(new TeamRace(teamId: teamId, raceId: race.Id));
           
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUpcomingLeagueRaces(int teamId, int leagueId)
        {
            var team = await _context.Teams
                                .Include(t => t.TeamRaces)
                                    .ThenInclude(tr => tr.Race)
                                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new ArgumentException();

            var upcomingLeagueTeamRaces = team.TeamRaces.Where(tr => tr.Race.LeagueId == leagueId).ToList();

            _context.TeamRaces.RemoveRange(upcomingLeagueTeamRaces);

            await _context.SaveChangesAsync();
        }


    }
}
