using System.ComponentModel.DataAnnotations;

namespace FiresportCalendar.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Display(Name = "Název")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Místo")]
        public string Place { get; set; } = string.Empty;
        [Display(Name = "Datum a čas")]
        public DateTime DateTime { get; set; }

        public IList<EventPerson> EventPeople { get; set; } = new List<EventPerson>();
    }
}
