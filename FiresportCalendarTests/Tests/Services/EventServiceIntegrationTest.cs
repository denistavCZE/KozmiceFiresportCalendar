using FiresportCalendar.Data;
using FiresportCalendar.Models;
using FiresportCalendar.Services;
using Microsoft.EntityFrameworkCore;
namespace FiresportCalendar.Tests.Tests.Services
{
    public class EventServiceIntegrationTest : IClassFixture<CustomWebApplicationFactory>
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
        public async Task AddEvent_Should_Save_And_Return_Event()
        {
            using var context = CreateContext();
            var service = new EventService(context);
            var time = DateTime.Today.AddDays(1);

            var @event = new Event
            {
                Name = "Test Event",
                Place = "Hlučín",
                DateTime = time
            };

            await service.AddEvent(@event);

            var saved = await service.GetEventById(@event.Id);

            Assert.NotNull(saved);
            Assert.Equal("Test Event", saved.Name);
            Assert.Equal("Hlučín", saved.Place);
            Assert.Equal(time, saved.DateTime);
        }
        [Fact]
        public async Task AddEventPerson_Should_Create_Record()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var @event = new Event { Name = "Test Event", Place = "Hlučín", DateTime = DateTime.Today.AddDays(1) };
            context.Events.Add(@event);

            var person = new Person { Id = "1", UserName = "Test person" };
            context.Users.Add(person);

            await context.SaveChangesAsync();

            await service.AddEventPerson(@event.Id, person.Id);

            var exists = await context.EventPeople
                .AnyAsync(ep => ep.EventId == @event.Id && ep.PersonId == person.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task GetEventsByIds_Should_Return_Only_Selected_Events_In_Order()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var event1 = new Event { Name = "Test Event1", Place = "Hlučín", DateTime = DateTime.Today.AddDays(3) };
            var event2 = new Event { Name = "Test Event2", Place = "Kozmice", DateTime = DateTime.Today.AddDays(1) };
            var event3 = new Event { Name = "Test Event3", Place = "Opava", DateTime = DateTime.Today.AddDays(2) };

            context.Events.AddRange(event1, event2, event3);
            await context.SaveChangesAsync();

            var result = await service.GetEventsByIds(new List<int> { event1.Id, event3.Id });

            Assert.Equal(2, result.Count);
            Assert.Equal("Test Event3", result[0].Name);
            Assert.Equal("Test Event1", result[1].Name);
        }


        [Fact]
        public async Task GetPersonEvents_Should_Return_EventIds_For_Person()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var event1 = new Event { Name = "Test Event1", Place = "Hlučín", DateTime = DateTime.Today.AddDays(1) };
            var event2 = new Event { Name = "Test Event2", Place = "Kozmice", DateTime = DateTime.Today.AddDays(2) };
            var person = new Person { Id = "1", UserName = "Test User" };

            context.Events.AddRange(event1, event2);
            context.Users.Add(person);
            await context.SaveChangesAsync();

            context.EventPeople.Add(new EventPerson(event1.Id, person.Id));
            await context.SaveChangesAsync();

            var result = await service.GetPersonEvents(person.Id);

            Assert.Single(result);
            Assert.Contains(event1.Id, result);
        }


        [Fact]
        public async Task GetEventDetail_Should_Return_Usernames()
        {
            using var context = CreateContext();
            var service = new EventService(context);
            var time = DateTime.Today.AddDays(1);

            var @event = new Event { Name = "Test Event", Place = "Hlučín", DateTime = time };
            var person1 = new Person { Id = "1", UserName = "Test person1" };
            var person2 = new Person { Id = "2", UserName = "Test person2" };

            context.Events.Add(@event);
            context.Users.Add(person1);
            context.Users.Add(person2);
            await context.SaveChangesAsync();

            context.EventPeople.Add(new EventPerson(@event.Id, person1.Id));
            context.EventPeople.Add(new EventPerson(@event.Id, person2.Id));
            await context.SaveChangesAsync();

            var result = await service.GetEventDetail(@event.Id);

            Assert.NotNull(result);
            Assert.Contains("Test person1", result.People);
            Assert.Contains("Test person2", result.People);
            Assert.Equal("Test Event", result.Event.Name);
            Assert.Equal("Hlučín", result.Event.Place);
            Assert.Equal(time, result.Event.DateTime);
        }
        [Fact]
        public async Task UpdateEventAsync_Should_Save_And_Return_Updated_And_Keep_People()
        {

            using var context = CreateContext();
            var service = new EventService(context);
            var time = DateTime.Today.AddDays(1);

            var @event = new Event { Name = "Test Event", Place = "Hlučín", DateTime = time };

            await service.AddEvent(@event);

            
            var person1 = new Person { Id = "1", UserName = "Test person1" };
            var person2 = new Person { Id = "2", UserName = "Test person2" };

            context.Users.Add(person1);
            context.Users.Add(person2);
            await context.SaveChangesAsync();


            context.EventPeople.Add(new EventPerson(@event.Id, person1.Id));
            context.EventPeople.Add(new EventPerson(@event.Id, person2.Id));
            await context.SaveChangesAsync();

            @event.Name = "Test Event2";
            @event.Place = "Kozmice";
            @event.DateTime = @event.DateTime.AddDays(1);
          
            await service.UpdateEventAsync(@event);

            var result = await service.GetEventDetail(@event.Id);

            Assert.NotNull(result);
            Assert.Contains("Test person1", result.People);
            Assert.Contains("Test person2", result.People);
            Assert.Equal("Test Event2", result.Event.Name);
            Assert.Equal("Kozmice", result.Event.Place);
            Assert.Equal(time.AddDays(1), result.Event.DateTime);
        }

        [Fact]
        public async Task UpdateEventAsync_Should_Throw_When_Event_Not_Exists()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var nonExistingEvent = new Event
            {
                Id = 999,
                Name = "Does not exist",
                Place = "Nowhere",
                DateTime = DateTime.Today
            };

            await Assert.ThrowsAsync<Exception>(() =>
                service.UpdateEventAsync(nonExistingEvent));
        }

        [Fact]
        public async Task GetEventDetail_Should_Return_Null_When_Event_Not_Exists()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var result = await service.GetEventDetail(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveEventPerson_Should_Not_Throw_When_Record_Not_Exists()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var @event = new Event { Name = "Test", Place = "Hlučín", DateTime = DateTime.Today.AddDays(1) };
            var person = new Person { Id = "1", UserName = "Test person" };

            context.Events.Add(@event);
            context.Users.Add(person);
            await context.SaveChangesAsync();

            var exception = await Record.ExceptionAsync(() =>
                service.RemoveEventPerson(@event.Id, person.Id));

            Assert.Null(exception);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Not_Throw_When_Event_Not_Exists()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var exception = await Record.ExceptionAsync(() =>
                service.DeleteByIdAsync(999));

            Assert.Null(exception);
        }

        [Fact]
        public async Task EventExists_Should_Return_False_When_Not_Exists()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var exists = await service.EventExists(999);

            Assert.False(exists);
        }

        [Fact]
        public async Task GetEvents_Should_Not_Return_Past_Events()
        {
            using var context = CreateContext();
            var service = new EventService(context);

            var pastEvent = new Event
            {
                Name = "Old Event Name",
                Place = "Old Event Place",
                DateTime = DateTime.Today.AddDays(-1)
            };

            var futureEvent = new Event
            {
                Name = "New Event Name",
                Place = "New Event Place",
                DateTime = DateTime.Today.AddDays(1)
            };

            context.Events.AddRange(pastEvent, futureEvent);
            await context.SaveChangesAsync();

            var events = await service.GetEvents();

            Assert.Single(events);
            Assert.Equal("New Event Name", events.First().Name);
        }



    }
}