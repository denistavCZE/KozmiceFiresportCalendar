using System.ComponentModel.DataAnnotations;

namespace FiresportCalendar.Models
{
    public class League
    {
        public int Id { get; set; }
        [Display(Name="Název")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Barva")]
        public ColorEnum Color { get; set; }
        public IList<Race> Races { get; set; } = new List<Race>();
        public IList<Team> Teams { get; set; } = new List<Team>();
    }
}
