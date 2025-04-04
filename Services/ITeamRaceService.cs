using FiresportCalendar.Models;
using System.Diagnostics;

namespace FiresportCalendar.Services
{
    public interface ITeamRaceService
    {
        Task<List<TeamRace>> GetUpcomingTeamRacesByTeamId(int teamId);
        Task<List<TeamRace>> GetAllTeamRacesByTeamId(int teamId);

        Task<List<TeamRace>> GetAllTeamRacesByRaceId(int raceId);
        Task<List<TeamRace>> GetUpcomingTeamRacesByRaceId(int raceId);

        Task<List<TeamRacePerson>> GetTeamRacePeople(int raceId, int teamId);
        Task<List<TeamRace>> SetTeamRacePeople(int teamId, int raceId, string kos, string spoj, string stroj, string becka, string rozdel, string lp, string pp);
        Task<bool> SetTeamRacePerson(int teamId, int raceId, int positionId, string personId);
        Task UnsetTeamRacePerson(int teamId, int raceId, int positionId, string personId);

    }
}
