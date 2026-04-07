using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface IEventService
    {
        Task<Event?> GetByIdAsync(int eventId);    
        Task<List<Event>> GetAllAsync();
        Task<List<Event>> GetByIdsAsync(List<int> eventIds);
        Task AddPersonAsync(int eventId, string personId);
        Task RemovePersonAsync(int eventId, string personId);
        Task<List<int>> GetByPersonIdAsync(string personId);
        Task<EventDetailModel?> GetDetailByIdAsync(int eventId);
        Task AddAsync(Event @event);
        Task UpdateAsync(Event model);
        Task DeleteByIdAsync(int id);
        Task<bool> ExistsAsync(int id);



    }
}
