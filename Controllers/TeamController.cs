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

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Member")]
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITeamService _teamService;
        private readonly IRaceService _raceService;
        private readonly ITeamRaceService _teamRaceService;
        private readonly IPersonService _personService;
        private readonly UserManager<Person> _userManager;

        public TeamController(
            ApplicationDbContext context,
            ITeamService teamService,
            IRaceService raceService,
            ITeamRaceService teamRaceService,
            IPersonService personService,
            UserManager<Person> userManager)
        {
            _context = context;
            _teamService = teamService;
            _raceService = raceService;
            _teamRaceService = teamRaceService;
            _personService = personService;
            _userManager = userManager;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            return View(await _teamService.GetTeams());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Races (int id)
        {
            var model = new RaceListModel();

            model.TeamRaces = await _teamRaceService.GetTeamRacesByTeamId(id);

            foreach(var tr in model.TeamRaces)
            {
                tr.Race = await _raceService.GetRaceById(tr.RaceId) ?? new Race();

                if(tr.Race.LeagueId.HasValue)
                    tr.Race.League = await _raceService.GetLeague(tr.Race.LeagueId.Value);

                tr.TeamRacePeople = await _teamRaceService.GetTeamRacePeople(tr.RaceId, tr.TeamId);
                foreach(var person in tr.TeamRacePeople)
                {
                    person.Person = await _personService.GetPersonById(person.PersonId);
                }
            }
            

            ViewBag.TeamId = id;

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RacesAdmin(int id)
        {
            var model = new RaceListModel();

            model.TeamRaces = await _teamRaceService.GetTeamRacesByTeamId(id);

            foreach (var tr in model.TeamRaces)
            {
                tr.Race = await _raceService.GetRaceById(tr.RaceId) ?? new Race();
                if (tr.Race.LeagueId.HasValue)
                    tr.Race.League = await _raceService.GetLeague(tr.Race.LeagueId.Value);
                tr.TeamRacePeople = await _teamRaceService.GetTeamRacePeople(tr.RaceId, tr.TeamId);
            }
            model.AllPeople = await _personService.GetPeople();

            ViewBag.TeamId = id;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRacePositions([FromForm] int teamId, [FromForm] int raceId, [FromForm] string kos, [FromForm] string spoj, [FromForm] string stroj, [FromForm] string becka, [FromForm] string rozdel, [FromForm] string lp, [FromForm] string pp)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null)
                await _teamRaceService.SetTeamRacePeople(teamId: teamId, raceId: raceId, kos: kos, spoj: spoj, stroj: stroj, becka: becka, rozdel: rozdel, lp: lp, pp: pp);
            return RedirectToAction("RacesAdmin", new { id = teamId });
        }

        [HttpPost]
        public async Task ConfirmRace(int teamId, int raceId, int positionId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(personId != null) 
                await _teamRaceService.SetTeamRacePerson(teamId, raceId, positionId, personId);
        }
        [HttpPost]
        public async Task DeclineRace(int teamId, int raceId, int positionId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null)
                await _teamRaceService.UnsetTeamRacePerson(teamId, raceId, positionId, personId);
        }

        public async Task<IActionResult> TimerRaces()
        {
            var races = await _raceService.GetTimerRaces();
            foreach (var race in races) {
                if (race.LeagueId.HasValue)
                    race.League = await _raceService.GetLeague(race.LeagueId.Value);
            }
            return View(races);
        }


    }
}
