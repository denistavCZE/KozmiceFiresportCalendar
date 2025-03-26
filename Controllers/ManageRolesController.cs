using FiresportCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace FiresportCalendar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesController : Controller
    {
        private readonly UserManager<Person> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Person> _signInManager;
        private readonly IConfiguration _configuration;
        public ManageRolesController(UserManager<Person> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Person> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.Where(u => u.EmailConfirmed).OrderBy(u => u.UserName).ToList(); // Filtruje uživatele s potvrzeným emailem
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList(); 

            var userRoleViewModels = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); 

                userRoleViewModels.Add(new UserRoleViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    UserRoles = roles.ToList(),
                    AllRoles = allRoles
                });
            }
            ViewData["SuperAdminEmail"] = _configuration["SuperAdminEmail"];

            return View(userRoleViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if(user.Email == _configuration["SuperAdminEmail"])
            {
                return NotFound("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName); 
            }
            else
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}