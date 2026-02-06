using FiresportCalendar.Data;
using FiresportCalendar.Models;
using FiresportCalendar.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace FiresportCalendar
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddDefaultIdentity<Person>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false; 
                options.Password.RequireUppercase = false;  
                options.Password.RequireNonAlphanumeric = false;  
                options.Password.RequiredLength = 1;  
                options.Password.RequiredUniqueChars = 0;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ·ËÔÈÏÌÚÛ¯öù˙˘˝û¡»œ…ÃÕ“”ÿäç⁄Ÿ›é";
                options.User.RequireUniqueEmail = true; 
            })
                .AddRoles<IdentityRole>()

                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            #region Services
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<IEmailSender, EmailService>();
            builder.Services.AddHttpClient<ReCaptchaService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IRaceService, RaceService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ITeamRaceService, TeamRaceService>();
            builder.Services.AddScoped<IPersonService, PersonService>();
            builder.Services.AddScoped<ILeagueService, LeagueService>();
            builder.Services.AddScoped<ICalendarService, CalendarService>();

            #endregion
            var app = builder.Build();
                
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();  // Apply any pending migrations

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await CreateRolesAsync(roleManager);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Member" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
