namespace FiresportCalendar.Models
{
    public class EventDetailModel
    {
        public Event Event { get; set; }
        public List<string> People { get; set; } = new List<string>();
    }
}
