namespace FiresportCalendar.Models
{
    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> UserRoles { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
