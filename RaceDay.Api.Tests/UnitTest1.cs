using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class UnitTest1
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task GetEvents_ReturnsEmptyList_WhenNoEventsExist()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EventsController(dbContext);

            // Act
            var result = await controller.GetEvents();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Event>>>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<Event>>(actionResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task CreateEvent_AddsEventToDatabase()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new EventsController(dbContext);

            var newEvent = new Event
            {
                EventName = "City Marathon",
                Location = "Cape Town",
                Distance = "42km",
                EventType = "Running",
                EventDate = DateTime.Now.AddDays(30),
                OrganizerId = 1
            };

            // Act
            var result = await controller.CreateEvent(newEvent);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdEvent = Assert.IsType<Event>(actionResult.Value);
            Assert.Equal("City Marathon", createdEvent.EventName);
            Assert.Equal(1, dbContext.Events.Count());
        }
    }
}