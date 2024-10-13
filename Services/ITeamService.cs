using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ITeamService
    {
        Task<Team?> GetTeamById(int teamId);
        Task<List<Team>> GetTeams();
        Task<List<Team>> GetActiveTeams();
        Task<bool> AddMember(int teamId, string personId);
        Task<bool> RemoveMember(int teamId, string personId);
        Task<bool> AddLeague(int teamId, int leagueId);
        Task<bool> RemoveLeague(int teamId, int leagueId);
        Task<bool> AddRace(int teamId, int raceId);
        Task<bool> RemoveRace(int teamId, int raceId);
        Task<List<Person>> GetMembers(int teamId);
        Task<bool> IsMember(int teamId, string personId);
        Task<List<League>> GetTeamLeagues(int teamId);
        Task AddUpcomingLeagueRaces(int teamId, int leagueId);
        Task RemoveUpcomingLeagueRaces(int teamId, int leagueId);


    }
}
