namespace FiresportCalendar.Models
{
    public class UserRoleViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> UserRoles { get; set; }
        public List<string> AllRoles { get; set; }
    }
}
