using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface IPersonService
    {
        Task<Person?> GetPersonById(string personId);
        Task<List<Person>> GetPeople();
    }
}
