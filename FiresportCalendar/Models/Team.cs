using System.ComponentModel.DataAnnotations;

namespace FiresportCalendar.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Display(Name="Název")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Aktivní")]
        public bool Active { get; set; } = true;
        public List<TeamRace> TeamRaces { get; set; } = new List<TeamRace>();
        public IList<Person> People { get; set; } = new List<Person>();
        public IList<League> Leagues { get; set; } = new List<League>();
    }
}
