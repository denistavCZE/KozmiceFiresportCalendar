using Microsoft.AspNetCore.Identity;

namespace FiresportCalendar.Models
{
    public class Person : IdentityUser
    {
        public IList<Team> Teams { get; set; } = new List<Team>();
        public IList<EventPerson> EventPeople { get; set; } = new List<EventPerson>();
        public List<TeamRacePerson> TeamRacePeople { get; set; } = new List<TeamRacePerson>();
    }
    
}
