using FiresportCalendar.Models;
using System.Diagnostics;

namespace FiresportCalendar.Services
{
    public interface ITeamRaceService
    {
        Task<List<TeamRace>> GetUpcomingByTeamIdAsync(int teamId);
        Task<List<TeamRace>> GetAllByTeamIdAsync(int teamId);

        Task<List<TeamRace>> GetAllByRaceIdAsync(int raceId);
        Task<List<TeamRace>> GetUpcomingByRaceIdAsync(int raceId);

        Task<List<TeamRacePerson>> GetPeopleAsync(int raceId, int teamId);
        Task<List<TeamRace>> SetPeopleAsync(int teamId, int raceId, string kos, string spoj, string stroj, string becka, string rozdel, string lp, string pp);
        Task<bool> AddPersonAsync(int teamId, int raceId, int positionId, string personId);
        Task RemovePersonAsync(int teamId, int raceId, int positionId, string personId);

    }
}
