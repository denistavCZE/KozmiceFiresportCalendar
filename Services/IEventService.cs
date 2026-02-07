using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface IEventService
    {
        Task<Event?> GetEventById(int eventId);    
        Task<List<Event>> GetEvents();
        Task<List<Event>> GetEventsByIds(List<int> eventIds);
        Task AddEventPerson(int eventId, string personId);
        Task RemoveEventPerson(int eventId, string personId);
        Task<List<int>> GetPersonEvents(string personId);
        Task<EventDetailModel?> GetEventDetail(int eventId);
        Task AddEvent(Event @event);
        Task UpdateEventAsync(Event model);
        Task DeleteByIdAsync(int id);
        Task<bool> EventExists(int id);



    }
}
