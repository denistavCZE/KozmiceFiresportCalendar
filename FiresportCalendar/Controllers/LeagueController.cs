using Microsoft.AspNetCore.Mvc;
using FiresportCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using FiresportCalendar.Services;
//using Microsoft.AspNetCore.Identity;

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LeagueController : Controller
    {
        private readonly ILeagueService _leagueService;

        public LeagueController(
            ILeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _leagueService.GetAllAsync());
        }

        //GET: Leagues/Detail/5
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EventDetailModel model = new EventDetailModel();

            var league = await _leagueService.GetByIdAsync(id.Value);
            if (league == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Leagues/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Leagues = await _leagueService.GetAllAsync();
            return View();
        }

        // POST: Leagues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(League league)
        {
            if (ModelState.IsValid)
            {
                await _leagueService.AddAsync(league);
                return RedirectToAction(nameof(Index));
            }
            return View(league);
        }

        // GET: Leagues/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _leagueService.GetByIdAsync(id.Value);
            ViewBag.Leagues = await _leagueService.GetAllAsync();
            if (league == null)
            {
                return NotFound();
            }
            return View(league);
        }

        // POST: Leagues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, League league)
        {
            if (id != league.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _leagueService.UpdateAsync(league);
                }
                catch (Exception)
                {
                    if (!LeagueExists(league.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _leagueService.DeleteByIdAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool LeagueExists(int id)
        {
            return (_leagueService.GetByIdAsync(id) == null) ? false : true ;
        }
        
    }
}
