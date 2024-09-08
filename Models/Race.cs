using Microsoft.Identity.Client;

namespace FiresportCalendar.Models
{
    public class Race
    {
        public int Id { get; set; }
        public string Place { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public bool Timer { get; set; } = false;
        public int? LeagueId { get; set; }
        public League? League { get; set; }
        public List<TeamRace> TeamRaces { get; set; } = new List<TeamRace>();
    
    }
}
