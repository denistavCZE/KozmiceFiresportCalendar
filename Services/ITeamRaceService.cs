using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ITeamRaceService
    {
        Task<List<TeamRace>> GetTeamRacesByTeamId(int teamId);
        Task<List<TeamRacePerson>> GetTeamRacePeople(int raceId, int teamId);
        Task<List<TeamRace>> SetTeamRacePeople(int teamId, int raceId, string kos, string spoj, string stroj, string becka, string rozdel, string lp, string pp);
        Task SetTeamRacePerson(int teamId, int raceId, int positionId, string personId);
        Task UnsetTeamRacePerson(int teamId, int raceId, int positionId, string personId);


    }
}
