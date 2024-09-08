namespace FiresportCalendar.Models
{
    public class TeamRacePerson
    {
        public int RaceId { get; set; }
        public int TeamId { get; set; }
        public string PersonId { get; set; } = string.Empty;

        public TeamRace TeamRace { get; set; }
        public Person Person { get; set; }

        public int Position { get; set; }

        public TeamRacePerson(int raceId, int teamId, string personId, int position)
        {
            RaceId = raceId;
            TeamId = teamId;
            PersonId = personId;
            Position = position;
        }
    }
}
