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
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Member")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;
        private readonly IPersonService _personService;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<Person> _userManager;

        public EventController(
            ApplicationDbContext context,
            IEventService eventService,
            IPersonService personService,
            ICalendarService calendarService,
            UserManager<Person> userManager)
        {
            _context = context;
            _eventService = eventService;
            _personService = personService;
            _calendarService = calendarService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportEvents(List<int> eventIds)
        {
            try
            {
                var events = await _eventService.GetEventsByIds(eventIds);
                var calendar = _calendarService.Export(events);

                var bytes = Encoding.UTF8.GetBytes(calendar);
                return File(bytes, "text/calendar", "events.ics");

            }
            catch (Exception e)
            {
                return BadRequest("Nepodařilo se exportovat akce.");
            }
        }

    }
}
