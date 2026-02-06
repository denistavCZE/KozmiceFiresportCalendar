using FiresportCalendar.Models;

namespace FiresportCalendar.Services
{
    public interface ICalendarService
    {
        public string Export(IEnumerable<Event> events);

        public string Export(IEnumerable<Race> races);

    }
}
