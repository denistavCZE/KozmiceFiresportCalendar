namespace FiresportCalendar.Models
{
    public class EventPerson
    {
        public int EventId { get; set; }
        public string PersonId { get; set; }

        public Event Event{ get; set; }
        public Person Person { get; set; }

       
        public EventPerson(int eventId, string personId)
        {
            EventId = eventId;
            PersonId = personId;
        }
    }
}
