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

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Member")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;
        private readonly IPersonService _personService;
        private readonly UserManager<Person> _userManager;

        public EventController(
            ApplicationDbContext context,
            IEventService eventService,
            IPersonService personService,
            UserManager<Person> userManager)
        {
            _context = context;
            _eventService = eventService;
            _personService = personService;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null)
                ViewBag.ConfirmedEvents = await _eventService.GetPersonEvents(personId);
            else
                ViewBag.ConfirmedEvents = new List<int>();
            return View(await _eventService.GetEvents());
        }

        // GET: Events/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EventDetailModel model = new EventDetailModel();

            var @event = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            model.Event = @event;

            var eventPeople = _context.EventPeople.Where(eu => eu.EventId == id).Select(eu => eu.PersonId).ToList();
            foreach(var person in eventPeople)
            {
                var username = (await _personService.GetPersonById(person))?.UserName;
                if(username != null)
                    model.People.Add(username);
            }

            return View(model);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
        //public async Task<IActionResult> Races (int id)
        //{
        //    var teamRaceList = await _teamRaceService.GetTeamRacesByTeamId(id);

        //    foreach(var tr in teamRaceList)
        //    {
        //        tr.Race = await _raceService.GetRaceById(tr.RaceId) ?? new Race();
        //        tr.Race.League = await _raceService.GetLeague(tr.Race.LeagueId);
        //        tr.TeamRaceUsers = await _teamRaceService.GetTeamRaceUsers(tr.RaceId, tr.TeamId);
        //        foreach(var user in tr.TeamRaceUsers)
        //        {
        //            user.User = await _userService.GetUserById(user.UserId);
        //        }
        //    }
        //    var model = new RaceListModel();
        //    model.TeamRaces = teamRaceList;

        //    ViewBag.TeamId = id;

        //    return View(model);
        //}
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> RacesAdmin(int id)
        //{

        //    var teamRaceList = await _teamRaceService.GetTeamRacesByTeamId(id);

        //    foreach (var tr in teamRaceList)
        //    {
        //        tr.Race = await _raceService.GetRaceById(tr.RaceId) ?? new Race();
        //        tr.Race.League = await _raceService.GetLeague(tr.Race.LeagueId);
        //        tr.TeamRaceUsers = await _teamRaceService.GetTeamRaceUsers(tr.RaceId, tr.TeamId);
        //    }
        //    var model = new RaceListModel();
        //    model.TeamRaces = teamRaceList;
        //    model.AllUsers = await _userService.GetUsers();

        //    ViewBag.TeamId = id;

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SetRacePositions([FromForm] int teamId, [FromForm] int raceId, [FromForm] string kos, [FromForm] string spoj, [FromForm] string stroj, [FromForm] string becka, [FromForm] string rozdel, [FromForm] string lp, [FromForm] string pp)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId != null)
        //        await _teamRaceService.SetTeamRaceUsers(teamId: teamId, raceId: raceId, kos: kos, spoj: spoj, stroj: stroj, becka: becka, rozdel: rozdel, lp: lp, pp: pp);
        //    return RedirectToAction("RacesAdmin", new { id = teamId });
        //}

        [HttpPost]
        public async Task ConfirmEvent(int eventId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(personId != null) 
                await _eventService.AddEventPerson(eventId, personId);
        }
        [HttpPost]
        public async Task DeclineEvent(int eventId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null)
                await _eventService.RemoveEventPerson(eventId, personId);
        }
    }
}
