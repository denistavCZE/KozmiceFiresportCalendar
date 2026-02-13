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
        private readonly IConfiguration _configuration;
        public ManageRolesController(UserManager<Person> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.OrderBy(u => u.UserName).ToList(); // Filtruje uživatele s potvrzeným emailem

            var userRoleViewModels = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userRoleViewModels.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    UserRoles = roles.ToList(),
                    EmailConfirmed = user.EmailConfirmed
                });
            }
            ViewData["SuperAdminEmail"] = _configuration["SuperAdminEmail"];
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(userRoleViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Uživatel byl úspěšně potvrzen";
            }
            else
            {
                TempData["StatusMessage"] = "Chyba při pokusu o potvrzení uživatele";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}