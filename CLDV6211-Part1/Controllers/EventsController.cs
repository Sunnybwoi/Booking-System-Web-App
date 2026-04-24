using CLDV6211_POE_PART1.Data;
using CLDV6211_POE_PART1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CLDV6211_Part1.Services;

namespace CLDV6211_POE_PART1.Controllers
{
    public class EventsController : Controller
    {
        private readonly CLDV6211_DbContext _context;
        private readonly IBlobService _blobService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(CLDV6211_DbContext context, IBlobService blobService, ILogger<EventsController> logger)
        {
            _context = context;
            _blobService = blobService;
            _logger = logger;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchQuery)
        {
            var events = _context.Events.Include(e => e.Venue).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                events = events.Where(e =>
                    e.Name.Contains(searchQuery) ||
                    (e.Venue != null && e.Venue.Name.Contains(searchQuery)));
                ViewBag.SearchQuery = searchQuery;
            }

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            var venues = _context.Venues
                .Select(v => new SelectListItem { Value = v.VenueID.ToString(), Text = v.VenueID + " (" + v.Name + ")" })
                .ToList();

            var model = new Models.ViewModels.EventFormViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                VenueSelectList = new SelectList(venues, "Value", "Text")
            };

            return View(model);
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.ViewModels.EventFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // --- Venue date-conflict check on Create ---
                // Prevent assigning a venue that already has another non-finished event
                // overlapping with the new event's date range.
                bool venueConflict = await _context.Events
                    .AnyAsync(e =>
                        e.VenueID == model.VenueID &&
                        e.EndDate > DateTime.Now &&                // ignore finished events
                        e.StartDate < model.EndDate &&             // overlap condition
                        model.StartDate < e.EndDate);              // overlap condition

                if (venueConflict)
                {
                    ModelState.AddModelError(string.Empty,
                        "This venue already has another active event scheduled during that period. " +
                        "Please choose a different venue or adjust the event dates.");
                    model.VenueSelectList = RebuildVenueList(model.VenueID);
                    return View(model);
                }

                var @event = new Event
                {
                    Name = model.Name,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    VenueID = model.VenueID
                };

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    try
                    {
                        @event.ImageURL = await _blobService.UploadEventImageAsync(model.ImageFile);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageURL", ex.Message);
                        model.VenueSelectList = RebuildVenueList(model.VenueID);
                        return View(model);
                    }
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["Error"] = string.Join(" | ", errors);

            model.VenueSelectList = RebuildVenueList(model.VenueID);
            return View(model);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var model = new Models.ViewModels.EventFormViewModel
            {
                EventID = @event.EventID,
                Name = @event.Name,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                VenueID = @event.VenueID,
                VenueSelectList = RebuildVenueList(@event.VenueID),
                ImageURL = @event.ImageURL
            };

            return View(model);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.ViewModels.EventFormViewModel model)
        {
            if (id != model.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Events.FindAsync(id);
                    if (existing == null) return NotFound();

                    // --- FIX: Venue date-conflict check on Edit ---
                    // This was completely missing before, allowing edits to create
                    // overlapping events at the same venue.
                    // The event being edited is excluded via EventID so it doesn't
                    // conflict with its own current dates.
                    bool venueConflict = await _context.Events
                        .AnyAsync(e =>
                            e.VenueID == model.VenueID &&
                            e.EventID != model.EventID &&          // exclude this event itself
                            e.EndDate > DateTime.Now &&            // ignore finished events
                            e.StartDate < model.EndDate &&         // overlap condition
                            model.StartDate < e.EndDate);          // overlap condition

                    if (venueConflict)
                    {
                        ModelState.AddModelError(string.Empty,
                            "This venue already has another active event scheduled during that period. " +
                            "Please choose a different venue or adjust the event dates.");
                        model.VenueSelectList = RebuildVenueList(model.VenueID);
                        return View(model);
                    }

                    existing.Name = model.Name;
                    existing.StartDate = model.StartDate;
                    existing.EndDate = model.EndDate;
                    existing.VenueID = model.VenueID;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existing.ImageURL))
                            await _blobService.DeleteEventImageAsync(existing.ImageURL);

                        existing.ImageURL = await _blobService.UploadEventImageAsync(model.ImageFile);
                    }

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(model.EventID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            model.VenueSelectList = RebuildVenueList(model.VenueID);
            return View(model);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null) return NotFound();

            if (@event.Bookings.Any() && @event.EndDate > DateTime.Now)
            {
                TempData["Error"] = "Cannot delete this event because it has active bookings to OnGoing/Upcoming events.";
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event != null)
            {
                if (@event.Bookings.Any() && @event.EndDate > DateTime.Now)
                {
                    TempData["Error"] = "Cannot delete this event because it has active bookings to OnGoing/Upcoming events.";
                    return RedirectToAction(nameof(Index));
                }

                if (!string.IsNullOrEmpty(@event.ImageURL))
                    await _blobService.DeleteEventImageAsync(@event.ImageURL);

                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        private bool EventExists(int id) =>
            _context.Events.Any(e => e.EventID == id);

        /// <summary>
        /// Returns a populated SelectList of venues with the specified venue pre-selected.
        /// </summary>
        private SelectList RebuildVenueList(int selectedVenueId)
        {
            var venues = _context.Venues
                .Select(v => new SelectListItem
                {
                    Value = v.VenueID.ToString(),
                    Text = v.VenueID + " (" + v.Name + ")"
                })
                .ToList();
            return new SelectList(venues, "Value", "Text", selectedVenueId.ToString());
        }
    }
}

/* References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8.
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
 *
 * Anthropic (2026) Claude [AI assistant]. Available at: https://claude.ai (Accessed: 23 March 2026).
 * Prompt used: 'How do I get database contents to display always when I start/run the app C#'.
 */