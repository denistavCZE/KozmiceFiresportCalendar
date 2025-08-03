using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class TeamRaceService : ITeamRaceService
    {
        public readonly ApplicationDbContext _context;
        public TeamRaceService( ApplicationDbContext context) { 
            _context = context;
           
        }
        public async Task<List<TeamRace>> GetUpcomingTeamRacesByTeamId(int teamId)
        {
            return await _context.TeamRaces.Where(tr => tr.Team.Id == teamId && tr.Race.DateTime > DateTime.Today).OrderBy(tr => tr.Race.DateTime).ToListAsync();
        }
        public async Task<List<TeamRace>> GetAllTeamRacesByTeamId(int teamId)
        {
            return await _context.TeamRaces.Where(tr => tr.Team.Id == teamId).OrderBy(tr => tr.Race.DateTime).ToListAsync();
        }
        public async Task<List<TeamRace>> GetUpcomingTeamRacesByRaceId(int raceId)
        {
            return await _context.TeamRaces.Where(tr => tr.Race.Id == raceId && tr.Race.DateTime > DateTime.Today).OrderBy(tr => tr.Race.DateTime).ToListAsync();
        }
        public async Task<List<TeamRace>> GetAllTeamRacesByRaceId(int raceId)
        {
            return await _context.TeamRaces.Where(tr => tr.Race.Id == raceId).OrderBy(tr => tr.Race.DateTime).ToListAsync();
        }
        public async Task<List<TeamRacePerson>> GetTeamRacePeople(int raceId, int teamId)
        {
            return await _context.TeamRacePeople.Where(tr => tr.TeamId == teamId && tr.RaceId == raceId).ToListAsync();
        }
       

        public async Task<List<TeamRace>> SetTeamRacePeople(int teamId, int raceId, string kos, string spoj, string stroj, string becka, string rozdel, string lp, string pp)
        {

            List<TeamRacePerson> teamRacePeople = new List<TeamRacePerson>();
            if (kos != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: kos, position: 1));
            if (spoj != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: spoj, position: 2));
            if (stroj != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: stroj, position: 3));
            if (becka != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: becka, position: 4));
            if (rozdel != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: rozdel, position: 5));
            if (lp != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: lp, position: 6));
            if (pp != null)
                teamRacePeople.Add(new TeamRacePerson(teamId: teamId, raceId: raceId, personId: pp, position: 7));


            var oldPeople = await _context.TeamRacePeople.Where(tr => tr.TeamId == teamId && tr.RaceId == raceId).ToListAsync();
            _context.TeamRacePeople.RemoveRange(oldPeople);

            await _context.TeamRacePeople.AddRangeAsync(teamRacePeople);

            await _context.SaveChangesAsync();

            return await _context.TeamRaces.Where(r => r.Team.Id == teamId).ToListAsync();
        }
        public async Task<bool> AddTeamRacePerson(int teamId, int raceId, int positionId, string personId)
        {
            if (positionId != -1) // Celkovou neúčast může mít více lidí
            {
                var oldPerson = await _context.TeamRacePeople.Where(tr => tr.TeamId == teamId && tr.RaceId == raceId && tr.Position == positionId).FirstOrDefaultAsync();

                if (oldPerson != null)
                    return false;
            }

            var thisPerson = await _context.TeamRacePeople.Where(tr => tr.TeamId == teamId && tr.RaceId == raceId && tr.PersonId == personId).FirstOrDefaultAsync();

            if (thisPerson != null)
                _context.TeamRacePeople.Remove(thisPerson);

            TeamRacePerson newPerson = new TeamRacePerson(teamId: teamId, raceId: raceId, personId: personId, position: positionId);
            await _context.TeamRacePeople.AddAsync(newPerson);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task RemoveTeamRacePerson(int teamId, int raceId, int positionId, string personId)
        {
          
            var person = await _context.TeamRacePeople.Where(tr => tr.TeamId == teamId && tr.RaceId == raceId && tr.Position == positionId).FirstOrDefaultAsync();

            if (person != null)
                _context.TeamRacePeople.Remove(person);

            await _context.SaveChangesAsync();
         
        }


    }
}
