using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FiresportCalendar.Models
{
    public class Race
    {
        public int Id { get; set; }
        [Display(Name = "Místo")]
        public string Place { get; set; } = string.Empty;
        [Display(Name = "Datum a čas")]
        public DateTime DateTime { get; set; }
        [Display(Name = "Časomíra?")]
        public bool Timer { get; set; } = false;
        [Display(Name = "Liga")]
        public int? LeagueId { get; set; }
        public League? League { get; set; }
        public List<TeamRace> TeamRaces { get; set; } = new List<TeamRace>();
    
    }
}
