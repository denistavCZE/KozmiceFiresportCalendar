using FiresportCalendar.Models;
using System.Text;

namespace FiresportCalendar.Services
{
    public class CalendarService : ICalendarService
    {
        public string Export(IEnumerable<Event> events)
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//FiresportCalendar//CZ");

            foreach (var e in events)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:e{e.Id}@firesportcalendar.cz");
                sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTSTART:{e.DateTime.ToUniversalTime():yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTEND:{e.DateTime.AddHours(4).ToUniversalTime():yyyyMMddTHHmmssZ}");
                sb.AppendLine($"SUMMARY:{e.Name}");
                sb.AppendLine($"DESCRIPTION:{e.Name} - {e.Place} - {e.DateTime.ToString("dd.MM.yyyy - HH:mm")}");
                sb.AppendLine($"LOCATION:{e.Place}");
                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }
        public string Export(IEnumerable<Race> races)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//FiresportCalendar//CZ");

            foreach (var r in races)
            {
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:r{r.Id}@firesportcalendar.cz");
                sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTSTART:{r.DateTime.ToUniversalTime():yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTEND:{r.DateTime.AddHours(4).ToUniversalTime():yyyyMMddTHHmmssZ}");
                sb.AppendLine($"SUMMARY:{r.League?.Name} {r.Place}");
                sb.AppendLine($"DESCRIPTION:{r.League?.Name} {r.Place} - {r.DateTime.ToString("dd.MM.yyyy - HH:mm")}");
                sb.AppendLine($"LOCATION:{r.Place}");
                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }
    }
}
