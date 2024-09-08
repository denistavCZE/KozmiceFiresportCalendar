using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ITeamService
    {
        Task<List<Team>> GetTeams();
    }
}
