using FiresportCalendar.Models;
using FiresportCalendar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Member")]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICalendarService _calendarService;

        public EventController(
            IEventService eventService,
            ICalendarService calendarService)
        {
            _eventService = eventService;
            _calendarService = calendarService;
        }

        // GET: Events
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _eventService.GetEvents());
        }

        // GET: Events/Detail/5
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _eventService.GetEventDetail(id.Value);

            if (model == null) 
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Events/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                await _eventService.AddEvent(@event);

                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _eventService.GetEventById(id.Value);

            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                    await _eventService.UpdateEventAsync(@event);
                }
                catch (Exception)
                {
                    if (!await _eventService.EventExists(@event.Id))
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
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _eventService.GetEventById(id.Value);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventService.DeleteByIdAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task ConfirmEvent(int eventId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(personId != null) 
                await _eventService.AddEventPerson(eventId, personId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task DeclineEvent(int eventId)
        {
            var personId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (personId != null)
                await _eventService.RemoveEventPerson(eventId, personId);
        }

        [HttpPost]
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
