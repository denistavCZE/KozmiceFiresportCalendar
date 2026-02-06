using FiresportCalendar.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using FiresportCalendar.Data;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

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
            return await _context.Events.FirstAsync(e => e.Id == eventId);
        }
        public async Task<List<Event>> GetEvents()
        {
            return await _context.Events.Where(e => e.DateTime >= DateTime.Today).OrderBy(e => e.DateTime).ToListAsync();
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
            var person = await _context.EventPeople.Where(eu => eu.EventId == eventId && eu.PersonId == personId).FirstOrDefaultAsync();

            if (person != null)
                _context.EventPeople.Remove(person);

            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetPersonEvents(string personId)
        {
            return await _context.EventPeople.Where(eu => eu.PersonId == personId).Select(eu => eu.EventId).ToListAsync();
        }
    }
}
