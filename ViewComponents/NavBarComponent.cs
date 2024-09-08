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

        public NavBarViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
     

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // this is how to avoid error of can't convert IQueryable to IEnumerable
            List<Team> teams = await _context.Teams.ToListAsync();
            return View(teams);
        }
    }
}
