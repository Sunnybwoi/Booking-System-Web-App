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
    public class VenuesController : Controller // Controller for managing Venue entities, providing CRUD operations and interactions with the database context
    {
        private readonly CLDV6211_DbContext _context; // Database context for accessing Venue and related entities

        // Constructor that initializes the database context through dependency injection,
        // allowing the controller to interact with the database
        public VenuesController(CLDV6211_DbContext context)
        {
            _context = context;
        }

        // GET: Venue
        // Retrieves a list of all venues from the database, and passes it to the view for display
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        /* GET: Venue/Details/5
         * Retrieves the details of a specific venue based on the provided ID, and passes it to the view for display
         * Generated with assistance from Anthropic's (2026) Claude [AI assistant],
         * which provided the initial code structure and logic for handling the details view of a venue
         * Prompt: 'How do I get database contents to display always when I start/run the app C#', 23 March. */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        /* GET: Venue/Create
         * Prepares the view for creating a new venue by returning the view to the user
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        public IActionResult Create()
        {
            return View();
        }

        /* POST: Venue/Create
         * To protect from overposting attacks, enable the specific properties you want to bind to.
         * It handles the form submission for creating a new venue, validates the model, and if valid, 
         * adds the new venue to the database and saves changes
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [HttpPost] // Specifies that this action method should only handle HTTP POST requests, which is appropriate for form submissions that create new resources
        [ValidateAntiForgeryToken] // Validates the anti-forgery token to prevent Cross-Site Request Forgery (CSRF) attacks, ensuring that the form submission is legitimate and comes from the expected source
        public async Task<IActionResult> Create([Bind("VenueID,Name,Location,Capacity,ImageURL,CreatedAt")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        /* GET: Venue/Edit/5
         * It retrieves the details of a specific venue based on the provided ID, and passes it to the view for editing
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) // Checks if the provided ID is null, and if so, returns a NotFound result, indicating that the requested resource cannot be found or accessed without a valid ID
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id); // Asynchronously finds the venue with the specified ID in the database context, and assigns it to the variable 'venue'
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        /* POST: Venue/Edit/5
         * To protect from overposting attacks, enable the specific properties you want to bind to.
         * It handles the form submission for editing an existing venue, validates the model, and if valid, it updates the venue in the database and saves changes. 
         * It also includes error handling for concurrency issues that may arise when multiple users attempt to edit the same venue simultaneously.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,Name,Location,Capacity,ImageURL,CreatedAt")] Venue venue)
        {
            if (id != venue.VenueID) // Checks if the provided ID does not match the VenueID of the venue being edited, and if so, returns a NotFound result, indicating that the requested resource cannot be found or accessed with the provided ID
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try // Attempts to update the venue in the database context and save changes.
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) // Catches exceptions that occur when there is a concurrency conflict during the update operation, which can happen when multiple users attempt to edit the same venue at the same time
                {
                    if (!VenueExists(venue.VenueID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // If the venue still exists but there is a concurrency conflict, it rethrows the exception to be handled by higher-level error handling mechanisms, allowing for appropriate error responses or logging
                    }
                }
                return RedirectToAction(nameof(Index)); // After successfully updating the venue, it redirects the user to the Index action, which typically displays a list of all venues, allowing the user to see the updated information in the context of the entire list
            }
            return View(venue);
        }

        /* GET: Venue/Delete/5
         * It retrieves the details of a specific venue based on the provided ID, and passes it to the view for confirmation before deletion
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022).Version 17.8.
         */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueID == id);// Asynchronously finds the venue with the specified ID in the database context, and assigns it to the variable 'venue'
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        /* POST: Venue/Delete/5
         * It handles the form submission for confirming the deletion of a venue, finds the venue in the database, removes it, and saves changes. 
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [HttpPost, ActionName("Delete")]// Specifies that this action method should handle HTTP POST requests for the "Delete" action,
                                        // allowing it to process form submissions that confirm the deletion of a venue
        [ValidateAntiForgeryToken]// Validates the anti-forgery token to prevent Cross-Site Request Forgery (CSRF) attacks,
                                  // ensuring that the form submission for deleting a venue is legitimate and comes from the expected source
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);// Asynchronously finds the venue with the specified ID in the database context,
                                                            // and assigns it to the variable 'venue'
            if (venue != null)
            {
                _context.Venues.Remove(venue);// removes the venue from the database context, marking it for deletion when changes are saved
            }

            await _context.SaveChangesAsync();// Asynchronously saves the changes to the database, which will execute the deletion of the venue from the database
            return RedirectToAction(nameof(Index));
        }

        // A private helper method that checks if a venue with the specified ID exists in the database context,
        // returning true if it exists and false otherwise
        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueID == id);
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
