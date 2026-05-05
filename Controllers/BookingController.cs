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
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBookingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Events = await dbContext.Events.ToListAsync();
                ViewBag.Venues = await dbContext.Venues.ToListAsync();
                return View(viewModel);
            }

            var exists = await dbContext.Bookings.AnyAsync(b =>
                b.VenueId == viewModel.VenueId &&
                b.BookingDate.Date == viewModel.BookingDate.Date);

            if (exists)
            {
                ModelState.AddModelError("", "This venue is already booked for today.");

                ViewBag.Events = await dbContext.Events.ToListAsync();
                ViewBag.Venues = await dbContext.Venues.ToListAsync();

                return View(viewModel);
            }

            var booking = new Booking
            {
                EventID = viewModel.EventID,
                VenueId = viewModel.VenueId,
                BookingDate = viewModel.BookingDate
            };

            await dbContext.AddAsync(booking);
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
                    EF.Functions.Like(b.Venue.VenueName, $"%{searchString}%"));
            }

            var bookings = await bookingsQuery.ToListAsync();

            ViewBag.CurrentFilter = searchString;

            return View(bookings);
        }
    }
}
