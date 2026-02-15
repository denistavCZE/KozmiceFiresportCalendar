using FiresportCalendar.Data;
using FiresportCalendar.Migrations;
using FiresportCalendar.Models;
using FiresportCalendar.Services;
using Microsoft.EntityFrameworkCore;
namespace FiresportCalendar.Tests.Tests.Services
{
    public class RaceServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return context;
        }
        [Fact]
        public async Task AddRaceAsync_Should_Save_Race()
        {
            using var context = CreateContext();
            var service = new RaceService(context);
            var time = DateTime.Today.AddDays(1);

            var race = new Race
            {
                Place = "Hlučín",
                DateTime = time,
                Timer = true
            };

            await service.AddRaceAsync(race);

            var saved = await context.Races.FirstOrDefaultAsync();

            Assert.NotNull(saved);
            Assert.Equal("Hlučín", saved.Place);
            Assert.Equal(time, saved.DateTime);
            Assert.True(saved.Timer);
        }
        [Fact]
        public async Task GetAllRaces_Should_Return_Ordered_By_Date()
        {
            using var context = CreateContext();
            var service = new RaceService(context);

            var race1 = new Race { Place = "Hlučín", DateTime = DateTime.Today.AddDays(3) };
            var race2 = new Race { Place = "Kozmice", DateTime = DateTime.Today.AddDays(1) };

            context.Races.AddRange(race1, race2);
            await context.SaveChangesAsync();

            var result = await service.GetAllRaces();

            Assert.Equal(2, result.Count);
            Assert.Equal("Kozmice", result[0].Place);
            Assert.Equal("Hlučín", result[1].Place);
        }

        [Fact]
        public async Task GetAllUpcomingRaces_Should_Not_Return_Past_Races()
        {
            using var context = CreateContext();
            var service = new RaceService(context);

            var oldRace = new Race { Place = "Old Place", DateTime = DateTime.Today.AddDays(-1) };
            var futureRace = new Race { Place = "Future Place", DateTime = DateTime.Today.AddDays(2) };

            context.Races.AddRange(oldRace, futureRace);
            await context.SaveChangesAsync();

            var result = await service.GetAllUpcomingRaces();

            Assert.Single(result);
            Assert.Equal("Future Place", result[0].Place);
        }

        [Fact]
        public async Task GetRaceById_Should_Return_Race()
        {
            using var context = CreateContext();
            var service = new RaceService(context);
            var time = DateTime.Today;
            var race = new Race { Place = "Hlučín", DateTime = time, Timer = false };
            context.Races.Add(race);
            await context.SaveChangesAsync();

            var result = await service.GetRaceById(race.Id);

            Assert.NotNull(result);
            Assert.Equal("Hlučín", result.Place);
            Assert.Equal(time, result.DateTime);
            Assert.False(result.Timer);
        }

        [Fact]
        public async Task UpdateRaceAsync_Should_Throw_When_Not_Exists()
        {
            using var context = CreateContext();
            var service = new RaceService(context);

            var race = new Race
            {
                Id = 999,
                Place = "X",
                DateTime = DateTime.Today
            };

            await Assert.ThrowsAsync<Exception>(() =>
                service.UpdateRaceAsync(race));
        }

        [Fact]
        public async Task UpdateRaceAsync_Should_Update_Values()
        {
            using var context = CreateContext();
            var service = new RaceService(context);
            var time = DateTime.Today;
            var race = new Race { Place = "Hlučín", DateTime = time, Timer = false };

            context.Races.Add(race);
            await context.SaveChangesAsync();

            race.Place = "Kozmice";
            race.Timer = true;
            race.DateTime = time.AddDays(1);

            await service.UpdateRaceAsync(race);

            var updated = await context.Races.FindAsync(race.Id);

            Assert.NotNull(updated);
            Assert.Equal("Kozmice", updated.Place);
            Assert.Equal(time.AddDays(1), updated.DateTime);
            Assert.True(updated.Timer);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Remove_Race()
        {
            using var context = CreateContext();
            var service = new RaceService(context);

            var race = new Race { Place = "ToDel", DateTime = DateTime.Today };
            context.Races.Add(race);
            await context.SaveChangesAsync();

            await service.DeleteByIdAsync(race.Id);

            var exists = await context.Races.AnyAsync(r => r.Id == race.Id);

            Assert.False(exists);
        }

        [Fact]
        public async Task GetTimerRaces_Should_Return_Only_Timer_And_Upcoming()
        {
            using var context = CreateContext();
            var service = new RaceService(context);

            var race1 = new Race { Place = "Valid", Timer = true, DateTime = DateTime.Today.AddDays(1) };
            var race2 = new Race { Place = "Past", Timer = true, DateTime = DateTime.Today.AddDays(-1) };
            var race3 = new Race { Place = "NoTimer", Timer = false, DateTime = DateTime.Today.AddDays(1) };

            context.Races.AddRange(race1, race2, race3);
            await context.SaveChangesAsync();

            var result = await service.GetTimerRaces();

            Assert.Single(result);
            Assert.Equal("Valid", result[0].Place);
        }

    }
}