using FiresportCalendar.Data;
using FiresportCalendar.Models;
using FiresportCalendar.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiresportCalendar.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly ITeamService _teamService;
        public NavBarViewComponent(ApplicationDbContext context, ITeamService teamService)
        {
            _context = context;
            _teamService = teamService;
        }
     

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Team> teams = await _teamService.GetActiveTeams();
            return View(teams);
        }
    }
}
