namespace FiresportCalendar.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public IList<EventPerson> EventPeople { get; set; } = new List<EventPerson>();
    }
}
