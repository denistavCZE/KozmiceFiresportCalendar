using System.Reflection.Metadata.Ecma335;

namespace FiresportCalendar.Models
{
    public class TeamEditModel
    {
        public Team Team { get; set; } = new Team();
        public List<Person> NonMembers { get; set; } = new List<Person>();
        public List<League> NonTeamLeagues { get; set; } = new List<League>();
        public List<Race> NonTeamRaces { get; set; } = new List<Race>();
        public List<Person> AllPeople { get; set; } = new List<Person>();
    }
}
