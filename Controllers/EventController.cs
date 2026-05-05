using EventEase.Data;
using EventEase.Models;
using EventEase.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EventController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEventViewModel viewModel)
        {
            var evt = new Event
            {
                EventName = viewModel.EventName,
                EventDate = viewModel.EventDate,
                Description = viewModel.Description,
                VenueId = viewModel.VenueId
            };

            await dbContext.Events.AddAsync(evt);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List"); // Better UX
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            var eventsQuery = dbContext.Events
                .Include(e => e.Venue)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                eventsQuery = eventsQuery.Where(e =>
                    e.EventName.Contains(searchString) ||
                    (e.Venue != null && e.Venue.VenueName.Contains(searchString))
                );
            }

            var events = await eventsQuery.ToListAsync(); // USE FILTERED QUERY

            ViewBag.CurrentFilter = searchString;

            return View(events);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var evt = await dbContext.Events.FindAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            dbContext.Events.Remove(evt);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var evt = await dbContext.Events.FindAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            var viewModel = new AddEventViewModel
            {
                EventName = evt.EventName,
                EventDate = evt.EventDate,
                Description = evt.Description,
                VenueId = evt.VenueId
            };

            ViewBag.EventId = evt.EventId; // pass ID to view

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddEventViewModel viewModel)
        {
            var evt = await dbContext.Events.FindAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            evt.EventName = viewModel.EventName;
            evt.EventDate = viewModel.EventDate;
            evt.Description = viewModel.Description;
            evt.VenueId = viewModel.VenueId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}