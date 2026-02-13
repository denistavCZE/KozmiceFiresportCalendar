namespace FiresportCalendar.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public List<TeamRace> TeamRaces { get; set; } = new List<TeamRace>();
        public IList<Person> People { get; set; } = new List<Person>();
        public IList<League> Leagues { get; set; } = new List<League>();
    }
}
