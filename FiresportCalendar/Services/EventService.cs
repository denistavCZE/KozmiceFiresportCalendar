using FiresportCalendar.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;

namespace FiresportCalendar.Services
{
    public class EventService : IEventService
    {
        public readonly ApplicationDbContext _context;
        public EventService( ApplicationDbContext context) { 
            _context = context;
        }
        public async Task<Event?> GetEventById(int eventId)
        {
            return await _context.Events.FindAsync(eventId);
        }
        public async Task<List<Event>> GetEvents()
        {
            return await _context.Events.Include(e => e.EventPeople).Where(e => e.DateTime >= DateTime.Today).OrderBy(e => e.DateTime).ToListAsync();
        }
        public async Task<List<Event>> GetEventsByIds(List<int> eventIds)
        {
            return await _context.Events.Where(e => eventIds.Contains(e.Id)).OrderBy(e => e.DateTime).ToListAsync();
        }
        public async Task AddEventPerson(int eventId, string personId)
        {
            EventPerson newPerson = new EventPerson(eventId: eventId, personId: personId);
            await _context.EventPeople.AddAsync(newPerson);

            await _context.SaveChangesAsync();
        }
        public async Task RemoveEventPerson(int eventId, string personId)
        {
            var person = await _context.EventPeople.FirstOrDefaultAsync(eu => eu.EventId == eventId && eu.PersonId == personId);

            if (person != null)
                _context.EventPeople.Remove(person);

            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetPersonEvents(string personId)
        {
            return await _context.EventPeople.Where(eu => eu.PersonId == personId).Select(eu => eu.EventId).ToListAsync();
        }

        public async Task<EventDetailModel?> GetEventDetail(int eventId)
        {
            var @event = await _context.Events
                                            .Include(e => e.EventPeople)
                                            .ThenInclude(ep => ep.Person)
                                            .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null)
                return null;

            var model = new EventDetailModel
            {
                Event = @event,
                People = @event.EventPeople
                             .Select(ep => ep.Person.UserName ?? "")
                             .Where(u => !string.IsNullOrEmpty(u))
                             .ToList()
            };
            return model;
        }

        public async Task AddEvent(Event @event)
        {
            _context.Add(@event);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateEventAsync(Event model)
        {
            var @event = await _context.Events.FindAsync(model.Id);

            if (@event == null)
                throw new Exception("Akce nenalezena");

            @event.Name = model.Name;
            @event.Place = model.Place;
            @event.DateTime = model.DateTime;

            await _context.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> EventExists(int id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }
    }
}
