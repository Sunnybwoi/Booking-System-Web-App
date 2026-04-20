using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV6211_POE_PART1.Data;
using CLDV6211_POE_PART1.Models;

namespace CLDV6211_POE_PART1.Controllers
{
    public class BookingsController : Controller
    {
        private readonly CLDV6211_DbContext _context;

        public BookingsController(CLDV6211_DbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchQuery)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e != null ? e.Venue : null)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                bookings = bookings.Where(b =>
                    (b.CustomerName != null && b.CustomerName.Contains(searchQuery)) ||
                    (b.Event != null && b.Event.Name.Contains(searchQuery)) ||
                    (b.Event != null && b.Event.Venue != null && b.Event.Venue.Name.Contains(searchQuery)));
                ViewBag.SearchQuery = searchQuery;
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            var events = _context.Events
                .Select(e => new SelectListItem { Value = e.EventID.ToString(), Text = e.EventID + " (" + e.Name + ")" })
                .ToList();
            var model = new Models.ViewModels.BookingFormViewModel
            {
                EventSelectList = new SelectList(events, "Value", "Text")
            };

            return View(model);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,EventID,CustomerName,ContactInfo,Status,CreatedAt")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Load the selected event including its venue
                var selectedEvent = await _context.Events
                    .Include(e => e.Venue)
                    .FirstOrDefaultAsync(e => e.EventID == booking.EventID);

                // --- FIX 1: Duplicate customer booking check ---
                // Prevents the same customer (matched by ContactInfo) from booking the
                // same event more than once. Cancelled bookings are not counted.
                // This correctly allows the same customer to book DIFFERENT events.
                bool customerDuplicate = await _context.Bookings
                    .AnyAsync(b =>
                        b.EventID == booking.EventID &&
                        b.ContactInfo.ToLower() == booking.ContactInfo.ToLower() &&
                        b.Status != "Cancelled");

                if (customerDuplicate)
                {
                    ModelState.AddModelError(string.Empty,
                        "This customer already has an active booking for this event.");
                    return View(RebuildViewModel(booking));
                }

                if (selectedEvent != null)
                {
                    // --- FIX 2: Venue date-conflict check against the EVENTS table ---
                    // The previous code checked Bookings for overlap, which misses events
                    // assigned to the same venue that have no bookings yet.
                    // Correct approach: check the Events table directly for any other event
                    // at the same venue whose date range overlaps with the selected event,
                    // and which has not yet finished (EndDate > now).
                    bool venueConflict = await _context.Events
                        .AnyAsync(e =>
                            e.VenueID == selectedEvent.VenueID &&
                            e.EventID != selectedEvent.EventID &&       // exclude the event itself
                            e.EndDate > DateTime.Now &&                  // ignore finished events
                            e.StartDate < selectedEvent.EndDate &&       // overlap condition
                            selectedEvent.StartDate < e.EndDate);        // overlap condition

                    if (venueConflict)
                    {
                        ModelState.AddModelError(string.Empty,
                            "This venue already has another active event scheduled during that period. " +
                            "Please choose a different event or wait until the existing event has finished.");
                        return View(RebuildViewModel(booking));
                    }
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(RebuildViewModel(booking));
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            return View(RebuildViewModel(booking));
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.ViewModels.BookingFormViewModel model)
        {
            if (id != model.BookingID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Bookings.FindAsync(id);
                    if (existing == null) return NotFound();

                    var selectedEvent = await _context.Events
                        .Include(e => e.Venue)
                        .FirstOrDefaultAsync(e => e.EventID == model.EventID);

                    if (selectedEvent != null)
                    {
                        // --- FIX 2 (Edit): Venue conflict check against Events table ---
                        // Same logic as Create — checks Events directly, not Bookings.
                        // The current booking's own event is excluded via EventID comparison.
                        bool venueConflict = await _context.Events
                            .AnyAsync(e =>
                                e.VenueID == selectedEvent.VenueID &&
                                e.EventID != selectedEvent.EventID &&
                                e.EndDate > DateTime.Now &&
                                e.StartDate < selectedEvent.EndDate &&
                                selectedEvent.StartDate < e.EndDate);

                        if (venueConflict)
                        {
                            ModelState.AddModelError(string.Empty,
                                "This venue already has another active event scheduled during that period. " +
                                "Please choose a different event or wait until the existing event has finished.");
                            model.EventSelectList = RebuildEventList(model.EventID);
                            return View(model);
                        }

                        // --- FIX 1 (Edit): Duplicate customer check, excluding current booking ---
                        bool customerDuplicate = await _context.Bookings
                            .AnyAsync(b =>
                                b.EventID == model.EventID &&
                                b.ContactInfo.ToLower() == model.ContactInfo.ToLower() &&
                                b.BookingID != model.BookingID &&
                                b.Status != "Cancelled");

                        if (customerDuplicate)
                        {
                            ModelState.AddModelError(string.Empty,
                                "This customer already has an active booking for this event.");
                            model.EventSelectList = RebuildEventList(model.EventID);
                            return View(model);
                        }
                    }

                    existing.EventID = model.EventID;
                    existing.CustomerName = model.CustomerName;
                    existing.ContactInfo = model.ContactInfo;
                    existing.Status = model.Status;
                    existing.CreatedAt = model.CreatedAt;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Booking updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(model.BookingID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            model.EventSelectList = RebuildEventList(model.EventID);
            return View(model);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Booking deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        private bool BookingExists(int id) =>
            _context.Bookings.Any(e => e.BookingID == id);

        /// <summary>
        /// Builds a BookingFormViewModel from a Booking entity, repopulating the
        /// event dropdown. Used when returning the Create/Edit view after a validation error.
        /// </summary>
        private Models.ViewModels.BookingFormViewModel RebuildViewModel(Booking booking) =>
            new Models.ViewModels.BookingFormViewModel
            {
                BookingID = booking.BookingID,
                EventID = booking.EventID,
                CustomerName = booking.CustomerName,
                ContactInfo = booking.ContactInfo,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                EventSelectList = RebuildEventList(booking.EventID)
            };

        /// <summary>
        /// Returns a populated SelectList of events, with the specified event pre-selected.
        /// </summary>
        private SelectList RebuildEventList(int selectedEventId)
        {
            var events = _context.Events
                .Select(e => new SelectListItem
                {
                    Value = e.EventID.ToString(),
                    Text = e.EventID + " (" + e.Name + ")"
                })
                .ToList();
            return new SelectList(events, "Value", "Text", selectedEventId.ToString());
        }
    }
}

/* References
 * Anthropic (2026) Claude [AI assistant]. Available at: https://www.anthropic.com (Accessed: 23 March 2026).
 * Prompt: 'How do I get database contents to display always when I start/run the app C#'.
 *
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8.
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
 */