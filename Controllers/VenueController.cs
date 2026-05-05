using EventEase.Data;
using EventEase.Models;
using EventEase.Models.Entities;
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddVenueViewModel viewModel, IFormFile image)
        {
            var imageUrl = await blobService.UploadAsync(image);

            var venue = new Venue
            {
                VenueName = viewModel.VenueName,
                Location = viewModel.Location,
                Capacity = viewModel.Capacity,
                ImageUrl = imageUrl
            };

            await dbContext.Venues.AddAsync(venue);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List");
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
    }
}
