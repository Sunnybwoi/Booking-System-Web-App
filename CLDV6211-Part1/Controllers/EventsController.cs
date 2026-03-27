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

namespace CLDV6211_POE_PART1.Controllers
{
    public class EventsController : Controller // Controller for managing Event entities, providing CRUD operations and interactions with the database context
    {
        private readonly CLDV6211_DbContext _context; // Database context for accessing Event and related entities

        // Constructor that initializes the database context through dependency injection,
        // allowing the controller to interact with the database
        public EventsController(CLDV6211_DbContext context)
        {
            _context = context;
        }

        // GET: Events
        // Retrieves a list of all events from the database, including their associated venues, and passes it to the view for display
        public async Task<IActionResult> Index()
        {
            var cLDV6211_Part1Context = _context.Events.Include(e => e.Venue);
            return View(await cLDV6211_Part1Context.ToListAsync());
        }

        /* GET: Events/Details/5
         * Retrieves the details of a specific event based on the provided ID, including its associated venue, and passes it to the view for display.
         * If the ID is null or the event is not found, it returns a NotFound result.
         * Generated with assistance from Anthropic (2026) Claude [AI assistant].
         * Prompt: 'How do I get database contents to display always when I start/run the app C#', 23 March. */

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        /* GET: Events/Create
         * Prepares the view for creating a new event by populating a dropdown list of venues and returning the view to the user.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8. */
        public IActionResult Create()
        {
            var venues = _context.Venues
                .Select(v => new SelectListItem { Value = v.VenueID.ToString(), Text = v.VenueID + " (" + v.Name + ")" })
                .ToList();
            ViewData["VenueID"] = new SelectList(venues, "Value", "Text");
            return View();
        }

        /* POST: Events/Create
         * Handles the form submission for creating a new event.
         * It validates the model state, adds the new event to the database context, and saves the changes.
         * To protect from overposting attacks, enable the specific properties you want to bind to.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8. */

        [HttpPost]// Specifies that this action method should only handle HTTP POST requests, which is appropriate for form submissions that create new resources
        [ValidateAntiForgeryToken]// Validates the anti-forgery token to prevent CSRF attacks when handling the form submission for creating a new event
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate,VenueID")] Event @event)
        {
            if (ModelState.IsValid) 
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Surface validation errors to the view
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["Error"] = string.Join(" | ", errors);

            var venues = _context.Venues
                .Select(v => new SelectListItem { Value = v.VenueID.ToString(), Text = v.VenueID + " (" + v.Name + ")" })
                .ToList();
            ViewData["VenueID"] = new SelectList(venues, "Value", "Text", @event.VenueID.ToString());
            return View(@event);
        }

        /* GET: Events/Edit/5
         * Retrieves the event to be edited based on the provided ID and
         * prepares the view for editing by populating a dropdown list of venues.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8. */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // Checks if the provided ID is null and returns a NotFound result if it is
            {
                return NotFound();
            }
             
            var @event = await _context.Events.FindAsync(id); // Retrieves the event from the database context based on the provided ID
            if (@event == null) 
            {
                return NotFound();
            }

            // Prepares the view for editing by populating a dropdown list of venues, setting the selected value to the current venue of the event, and returns the view with the event data
            var venues = (await _context.Venues.ToListAsync())
                .Select(v => new SelectListItem { Value = v.VenueID.ToString(), Text = v.VenueID + " (" + v.Name + ")" })
                .ToList();
            ViewData["VenueID"] = new SelectList(venues, "Value", "Text", @event.VenueID.ToString()); 
            return View(@event);
        }

        /* POST: Events/Edit/5
         * Handles the form submission for editing an existing event.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8. */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Name,StartDate,EndDate,VenueID")] Event @event)
        {
            if (id != @event.EventID) // Checks if the provided ID does not match the EventID of the event being edited and returns a NotFound result if they do not match, ensuring that the correct event is being edited
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try // Attempts to update the event in the database context and save changes, handling potential concurrency issues
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)// Handles concurrency issues when multiple users attempt to edit the same event simultaneously
                {
                    if (!EventExists(@event.EventID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Rethrows the exception if the event still exists, allowing higher-level error handling to manage it
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirects the user to the Index action after successfully editing the event, allowing them to see the updated list of events
            }

            var venues = (await _context.Venues.ToListAsync())
                .Select(v => new SelectListItem { Value = v.VenueID.ToString(), Text = v.VenueID + " (" + v.Name + ")" })
                .ToList();
            ViewData["VenueID"] = new SelectList(venues, "Value", "Text", @event.VenueID.ToString()); // Prepares the view for editing again by repopulating the dropdown list of venues and setting the selected value to the current venue of the event,
                                                                                                                          // in case the model state is invalid and the user needs to correct errors before resubmitting  
            return View(@event);
        }

        /* GET: Events/Delete/5
         * Retrieves the event to be deleted based on the provided ID, including its associated venue,
         * and passes it to the view for confirmation before deletion.
         * Code completion assisted by Visual Studio IntelliSense
         *(Microsoft Corporation, 2022).Version 17.8. */

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);// Asynchronously finds the event with the specified ID in the database context,
                                                           // including its associated venue, and assigns it to the variable '@event'
            if (@event == null) 
            {
                return NotFound();
            }

            return View(@event);
        }

        /* POST: Events/Delete/5
         * Handles the form submission for confirming the deletion of an event.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8. */

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null) // Checks if the event exists before attempting to remove it from the database context and save changes
            {
                _context.Events.Remove(@event);// Removes the event from the database context, marking it for deletion when changes are saved
            }

            await _context.SaveChangesAsync();// Saves the changes to the database after removing the event, ensuring that the deletion is persisted
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if an event exists in the database based on the provided ID,
        // used for handling concurrency issues during editing
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}

/*References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8. 
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
 * 
 * Anthropic (2026) Claude [AI assistant]. Available at: https://claude.ai (Accessed: 23 March 2026). 
 * Prompt used: 'How do I get database contents to display always when I start/run the app C#'.
 */