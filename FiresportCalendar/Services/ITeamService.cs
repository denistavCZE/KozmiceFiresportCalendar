using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ITeamService
    {
        Task<Team?> GetByIdAsync(int teamId);
        Task<List<Team>> GetAllAsync();
        Task<List<Team>> GetActiveTeamsAsync();
        Task AddAsync(Team team);
        Task<bool> AddMemberAsync(int teamId, string personId);
        Task<bool> RemoveMemberAsync(int teamId, string personId);
        Task<bool> AddLeagueAsync(int teamId, int leagueId);
        Task<bool> RemoveLeagueAsync(int teamId, int leagueId);
        Task<bool> AddRaceAsync(int teamId, int raceId);
        Task<bool> RemoveRaceAsync(int teamId, int raceId);
        Task<List<Person>> GetMembersAsync(int teamId);
        Task<bool> IsMember(int teamId, string personId);
        Task<List<League>> GetLeaguesAsync(int teamId);
        Task AddUpcomingLeagueRacesAsync(int teamId, int leagueId);
        Task RemoveUpcomingLeagueRacesAsync(int teamId, int leagueId);
        Task ToggleActiveAsync(int teamId);
        Task DeleteByIdAsync(int id);

    }
}
