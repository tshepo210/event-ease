using EventEase.Data;
using EventEase.Models;
using EventEase.Models.Entities;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        private readonly BlobService blobService;
        public VenueController(ApplicationDbContext dbContext, BlobService blobService)
        {
            this.dbContext = dbContext;
            this.blobService = blobService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddVenueViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddVenueViewModel viewModel, IFormFile? image)
        {
            if (image == null || image.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Please select an image.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string imageUrl;

            try
            {
                imageUrl = await blobService.UploadAsync(image);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
                return View(viewModel);
            }


            var venue = new Venue
            {
                VenueName = viewModel.VenueName,
                Location = viewModel.Location,
                Capacity = viewModel.Capacity,
                ImageUrl = imageUrl
            };

            await dbContext.Venues.AddAsync(venue);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }
        [HttpGet]
        public async Task<IActionResult> List(string searchString)
        {
            var venuesQuery = dbContext.Venues.AsQueryable();
            // Apply filter if user typed something
            if (!string.IsNullOrEmpty(searchString))
            {
                venuesQuery = venuesQuery.Where(v =>
                    v.VenueName.ToString().Contains(searchString));
            }
            var venue = await venuesQuery.ToListAsync();

            ViewBag.CurrentFilter = searchString;

            return View(venue);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var v = await dbContext.Venues.FindAsync(id);

            if (v == null)
            {
                return NotFound();
            }

            // Check if venue has bookings
            var hasBookings = await dbContext.Bookings
                .AnyAsync(b => b.VenueId == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete this venue because it has existing bookings.";
                return RedirectToAction("List");
            }

            dbContext.Venues.Remove(v);
            await dbContext.SaveChangesAsync();

            TempData["Success"] = "Venue deleted successfully.";
            return RedirectToAction("List");
        }
    }
}
