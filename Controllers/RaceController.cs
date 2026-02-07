using Microsoft.AspNetCore.Mvc;
using FiresportCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using FiresportCalendar.Services;
//using Microsoft.AspNetCore.Identity;

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RaceController : Controller
    {
        private readonly IRaceService _raceService;
        private readonly ILeagueService _leagueService;

        public RaceController(
            IRaceService raceService,
            ILeagueService leagueService)
        {
            _raceService = raceService;
            _leagueService = leagueService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _raceService.GetAllUpcomingRaces());
        }

        //GET: Races/Detail/5
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EventDetailModel model = new EventDetailModel();

            var race = await _raceService.GetRaceById(id.Value);
            if (race == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Races/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Leagues = await _leagueService.GetAllLeagues();
            return View();
        }

        // POST: Races/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Race race)
        {
            if (ModelState.IsValid)
            {
                await _raceService.AddRaceAsync(race);
                return RedirectToAction(nameof(Index));
            }
            return View(race);
        }

        // GET: Races/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = await _raceService.GetRaceById(id.Value);
            ViewBag.Leagues = await _leagueService.GetAllLeagues();
            if (race == null)
            {
                return NotFound();
            }
            return View(race);
        }

        // POST: Races/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Race race)
        {
            if (id != race.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _raceService.UpdateRaceAsync(race);
                }
                catch (Exception)
                {
                    if (!RaceExists(race.Id))
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

        // POST: Races/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _raceService.DeleteByIdAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool RaceExists(int id)
        {
            return (_raceService.GetRaceById(id) == null) ? false : true ;
        }
        
    }
}
