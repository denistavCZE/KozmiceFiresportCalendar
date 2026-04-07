using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface IPersonService
    {
        Task<Person?> GetByIdAsync(string personId);
        Task<List<Person>> GetAllAsync();
    }
}
