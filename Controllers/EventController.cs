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
        public async Task<IActionResult> Add()
        {
            var viewModel = new AddEventViewModel
            {
                Venues = await dbContext.Venues.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddEventViewModel viewModel)
        {
            viewModel.Venues = await dbContext.Venues.ToListAsync();

            if (viewModel.EventDate.HasValue && viewModel.EventDate.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError("EventDate", "Event date cannot be in the past.");
            }

            if (viewModel.VenueId == null || viewModel.VenueId == 0)
            {
                ModelState.AddModelError("VenueId", "Please select a venue.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var evt = new Event
            {
                EventName = viewModel.EventName,
                Description = viewModel.Description,
                EventDate = viewModel.EventDate!.Value,
                VenueId = viewModel.VenueId!.Value
            };

            dbContext.Events.Add(evt);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            var eventsQuery = dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Booking)
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

            ViewBag.Venues = await dbContext.Venues.ToListAsync();

            var viewModel = new EventListViewModel
            {
                Events = events,
                Venues = await dbContext.Venues.ToListAsync()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var evt = await dbContext.Events.FindAsync(id);

            if (evt == null)
            {
                return NotFound();
            }

            // Check if event has bookings
            var hasBookings = await dbContext.Bookings
                .AnyAsync(b => b.EventID == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete this event because it has existing bookings.";
                return RedirectToAction("List");
            }

            dbContext.Events.Remove(evt);
            await dbContext.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully.";
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
            if (viewModel.VenueId == null)
            {
                ModelState.AddModelError("VenueId", "Please select a venue.");
            }

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

            if (!ModelState.IsValid)
            {
                viewModel.Venues = await dbContext.Venues.ToListAsync();
                return View(viewModel);
            }
            if (!viewModel.EventDate.HasValue || viewModel.EventDate.Value < DateTime.Today)
            {
                ModelState.AddModelError("EventDate", "Event date cannot be in the past.");
                viewModel.Venues = await dbContext.Venues.ToListAsync();
                return View(viewModel);
            }
            evt.EventName = viewModel.EventName;
            evt.EventDate = viewModel.EventDate.Value;
            evt.Description = viewModel.Description;
            evt.VenueId = viewModel.VenueId.Value;

            dbContext.Update(evt);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var events = await dbContext.Events
                .Include(e => e.Venue)
                .Include(e => e.Booking)
                .ToListAsync();

            var viewModel = new EventDashboardViewModel
            {
                Events = events
            };

            return View(viewModel);
        }
    }
}