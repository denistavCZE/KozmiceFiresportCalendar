using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;
using FiresportCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using FiresportCalendar.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.ComponentModel.Design;
using System.Text;

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Person> _userManager;
        private readonly IRaceService _raceService;
        private readonly ILeagueService _leagueService;

        public RaceController(
            ApplicationDbContext context,
            UserManager<Person> userManager,
            IRaceService raceService,
            ILeagueService leagueService)
        {
            _context = context;
            _userManager = userManager;
            _raceService = raceService;
            _leagueService = leagueService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _raceService.GetAllUpcomingRaces());
        }

        //GET: Races/Detail/5
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
        public async Task<IActionResult> Create()
        {
            ViewBag.Leagues = await _leagueService.GetAllLeagues();
            return View();
        }

        // POST: Races/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                catch (DbUpdateConcurrencyException)
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

        [Authorize(Roles = "Member")]
        [HttpGet("calendar.ics")]
        public async Task<IActionResult> GetCalendarFeed()
        {
            var races = await _raceService.GetAllUpcomingRaces();

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//Kozmice Calendar//CS");

            foreach (var race in races)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:{race.Id}@kozmicecalendar");
                sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTSTART:{race.DateTime:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTEND:{race.DateTime.AddHours(4):yyyyMMddTHHmmssZ}");
                sb.AppendLine($"SUMMARY:{race.League?.Name} {race.Place} {race.DateTime:HH:mm}");
                sb.AppendLine($"DESCRIPTION:{race.League?.Name} {race.Place} {race.DateTime:HH:mm}");
                sb.AppendLine($"LOCATION:{race.Place}");
               sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/calendar; charset=utf-8", "calendar.ics");
        }

        private bool RaceExists(int id)
        {
            return _context.Races.Any(r => r.Id == id);
        }
        
    }
}
