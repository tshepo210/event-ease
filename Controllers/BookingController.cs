using EventEase.Data;
using EventEase.Models;
using EventEase.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public BookingController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = new AddBookingViewModel
            {
                Events = await dbContext.Events.ToListAsync(),
                Venues = await dbContext.Venues.ToListAsync()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBookingViewModel viewModel)
        {
            ViewBag.Events = await dbContext.Events.ToListAsync();
            ViewBag.Venues = await dbContext.Venues.ToListAsync();

            if (viewModel.EventID == 0)
            {
                ModelState.AddModelError("EventID", "Please select an event.");
            }

            if (viewModel.VenueId == 0)
            {
                ModelState.AddModelError("VenueId", "Please select a venue.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Get event
            var selectedEvent = await dbContext.Events
                .FirstOrDefaultAsync(e => e.EventId == viewModel.EventID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Invalid event selected.");
                return View(viewModel);
            }

            // Prevent double booking
            var exists = await dbContext.Bookings
                .Include(b => b.Event)
                .AnyAsync(b =>
                    b.VenueId == viewModel.VenueId &&
                    b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (exists)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                return View(viewModel);
            }

            // Create booking
            var booking = new Booking
            {
                EventID = viewModel.EventID,
                VenueId = viewModel.VenueId,
                BookingDate = DateTime.Now
            };

            await dbContext.Bookings.AddAsync(booking);

            // Auto update event's VenueId when a booking is made
            selectedEvent.VenueId = viewModel.VenueId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
        }
        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            var bookingsQuery = dbContext.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookingsQuery = bookingsQuery.Where(b =>
                    EF.Functions.Like(b.Event.EventName, $"%{searchString}%") ||
                    EF.Functions.Like(b.Venue.VenueName, $"%{searchString}%") ||
                    EF.Functions.Like(b.BookingID.ToString(), $"%{searchString}%"));
            }

            var bookings = await bookingsQuery.ToListAsync();

            ViewBag.CurrentFilter = searchString;

            return View(bookings);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await dbContext.Bookings.FindAsync(id);

            if (b == null)
            {
                return NotFound();
            }

            dbContext.Bookings.Remove(b);
            await dbContext.SaveChangesAsync();

            TempData["Success"] = "Booking deleted successfully.";
            return RedirectToAction("List");
        }
    }
}
