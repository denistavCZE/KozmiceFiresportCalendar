using FiresportCalendar.Data;
using FiresportCalendar.Models;
using FiresportCalendar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        private readonly ILeagueService _leagueService;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<Person> _userManager;

        public TeamController(
            ApplicationDbContext context,
            ITeamService teamService,
            IRaceService raceService,
            ITeamRaceService teamRaceService,
            IPersonService personService,
            ILeagueService leagueService,
            ICalendarService calendarService,
            UserManager<Person> userManager)
        {
            _context = context;
            _teamService = teamService;
            _raceService = raceService;
            _teamRaceService = teamRaceService;
            _personService = personService;
            _leagueService = leagueService;
            _calendarService = calendarService;
            _userManager = userManager;
        }

        // GET: Teams
        [Authorize(Roles="Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _teamService.GetTeams());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(int teamId, string personId)
        {
            if (await _teamService.AddMember(teamId, personId))
                return RedirectToAction("Edit", new { id = teamId });
            else
                return RedirectToAction("Error", "Home");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int teamId, string personId)
        {
            if (await _teamService.RemoveMember(teamId, personId))
                return RedirectToAction("Edit", new { id = teamId });
            else
                return RedirectToAction("Error", "Home");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeague(int teamId, int leagueId)
        {
            if (await _teamService.AddLeague(teamId, leagueId))
            {
                await _teamService.AddUpcomingLeagueRaces(teamId, leagueId);
                
                return RedirectToAction("Edit", new { id = teamId });
            }
            else
                return RedirectToAction("Error", "Home");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLeague(int teamId, int leagueId)
        {
            if (await _teamService.RemoveLeague(teamId, leagueId))
            {
                await _teamService.RemoveUpcomingLeagueRaces(teamId: teamId, leagueId: leagueId);
                
                return RedirectToAction("Edit", new { id = teamId });
            }
            else
                return RedirectToAction("Error", "Home");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRace(int teamId, int raceId)
        {
            if (await _teamService.AddRace(teamId, raceId))
                return RedirectToAction("Edit", new { id = teamId });
            else
                return RedirectToAction("Error", "Home");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRace(int teamId, int raceId)
        {
            if (await _teamService.RemoveRace(teamId, raceId))
                return RedirectToAction("Edit", new { id = teamId });
            else
                return RedirectToAction("Error", "Home");
        }

        // GET: Teams/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var team = await _teamService.GetTeamById(id);
            if (team == null)
            {
                return NotFound();
            }

            team.TeamRaces = team.TeamRaces.OrderBy(tr => tr.Race.DateTime).ToList();

            var model = new TeamEditModel();
            model.Team = team;
            model.AllPeople = await _personService.GetPeople();
            model.NonMembers = model.AllPeople.Where(p => !team.People.Contains(p)).ToList();
            model.NonTeamLeagues = (await _leagueService.GetAllLeagues()).Where(l => !team.Leagues.Contains(l)).ToList();
            model.NonTeamRaces = (await _raceService.GetAllUpcomingRaces()).Where(r => !team.TeamRaces.Select(tr => tr.RaceId).Contains(r.Id)).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail (int id)
        {
            Team? team = await _teamService.GetTeamById(id);

            if (team == null)
                return NotFound();
            
            team.TeamRaces = team.TeamRaces.OrderBy(tr => tr.Race.DateTime).ToList();

            return View(team);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRacePositions([FromForm] int teamId, [FromForm] int raceId, [FromForm] string kos, [FromForm] string spoj, [FromForm] string stroj, [FromForm] string becka, [FromForm] string rozdel, [FromForm] string lp, [FromForm] string pp)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null)
                await _teamRaceService.SetTeamRacePeople(teamId: teamId, raceId: raceId, kos: kos, spoj: spoj, stroj: stroj, becka: becka, rozdel: rozdel, lp: lp, pp: pp);
            return RedirectToAction("Edit", new { id = teamId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRace(int teamId, int raceId, int positionId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (personId != null && await _teamService.IsMember(teamId, personId))
                    if (!await _teamRaceService.AddTeamRacePerson(teamId, raceId, positionId, personId))
                        return BadRequest("Tuto pozici už potvrdil někdo jiný.");
            }
            catch (Exception ex)
            {
                return BadRequest("Došlo k chybě na serveru.\nZkuste akci opakovat, případně se obraťte na administrátora.");
            }
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task DeclineRace(int teamId, int raceId, int positionId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null && await _teamService.IsMember(teamId, personId))
                await _teamRaceService.RemoveTeamRacePerson(teamId, raceId, positionId, personId);
        }

        [HttpGet]
        public async Task<IActionResult> TimerRaces()
        {
            var races = await _raceService.GetTimerRaces();
            foreach (var race in races) {
                if (race.LeagueId.HasValue)
                    race.League = await _leagueService.GetLeagueById(race.LeagueId.Value);
            }
            return View(races);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _teamService.ToggleActive(id);

            return RedirectToAction("Index", id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportRaces(List<int> raceIds)
        {
            try
            {
                var races = await _raceService.GetRacesByIds(raceIds);
                var calendar = _calendarService.Export(races);

                var bytes = Encoding.UTF8.GetBytes(calendar);
                return File(bytes, "text/calendar", "races.ics");

            }
            catch (Exception e)
            {
                return BadRequest("Nepodařilo se exportovat závody.");
            }
        }

    }
}
