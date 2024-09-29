namespace FiresportCalendar.Models
{
    public class TeamRace
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int RaceId { get; set; }
        public Race Race{ get; set; }

        public List<TeamRacePerson> TeamRacePeople { get; set; } = new List<TeamRacePerson>();
        
        public TeamRace(int teamId, int raceId)
        {
            //Team = team;
            TeamId = teamId;
            //Race = race;
            RaceId = raceId;
        }
    }
}
