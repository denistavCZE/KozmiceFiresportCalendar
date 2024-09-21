namespace FiresportCalendar.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IList<Race> Races { get; set; } = new List<Race>();
        public IList<Team> Teams { get; set; } = new List<Team>();
    }
}
