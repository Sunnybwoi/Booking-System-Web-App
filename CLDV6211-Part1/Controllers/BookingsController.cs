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
    public class BookingsController : Controller // Controller class provides methods for handling HTTP requests and generating responses, typically by rendering views or returning data.
    {
        private readonly CLDV6211_DbContext _context;// This field holds a reference to the database context, which is used to interact with the database.
                                                     // It allows the controller to perform CRUD operations on the Booking entities.

        // The constructor takes a CLDV6211_DbContext as a parameter and assigns it to the _context field.
        public BookingsController(CLDV6211_DbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        // This action method retrieves a list of all bookings from the database, including related event data, and passes it to the view for display.
        public async Task<IActionResult> Index()
        {
            var cLDV6211_Part1Context = _context.Bookings.Include(b => b.Event);
            return View(await cLDV6211_Part1Context.ToListAsync());
        }

        /* GET: Bookings/Details/5
         * It retrieves a specific booking based on the provided ID, including related event data, and passes it to the view for display.
         * Generated with assistance from Anthropic's (2026) Claude [AI assistant],
         * which provided the initial code structure and logic for handling the details view of a venue
         * Prompt: 'How do I get database contents to display always when I start/run the app C#', 23 March.
         */
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(m => m.BookingID == id);// Asynchronously retrieves the booking with the specified ID from the database context,
                                                             // including related event data, and assigns it to the variable 'booking'
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        /* GET: Bookings/Create
         * It prepares the view for creating a new booking by returning the view to the user
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        public IActionResult Create()
        {
            var events = _context.Events
                .Select(e => new SelectListItem { Value = e.EventID.ToString(), Text = e.EventID + " (" + e.Name + ")" })
                .ToList();
            ViewData["EventID"] = new SelectList(events, "Value", "Text");
            return View();
        }

        /* POST: Bookings/Create
         * To protect from overposting attacks, enable the specific properties you want to bind to.
         * It handles the form submission for creating a new venue, validates the model, and if valid, 
         * adds the new venue to the database and saves changes
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [HttpPost]// Specifies that this action method should only handle HTTP POST requests, which is appropriate for form submissions that create new resources
        [ValidateAntiForgeryToken]// Validates the anti-forgery token to prevent Cross-Site Request Forgery (CSRF) attacks, ensuring that the form submission is legitimate and comes from the expected source
        public async Task<IActionResult> Create([Bind("BookingID,EventID,CustomerName,ContactInfo,Status,CreatedAt")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var events = _context.Events
                .Select(e => new SelectListItem { Value = e.EventID.ToString(), Text = e.EventID + " (" + e.Name + ")" })
                .ToList();
            ViewData["EventID"] = new SelectList(events, "Value", "Text", booking.EventID.ToString());
            return View(booking);
        }

        /* GET: Bookings/Edit/5
         * It retrieves the details of a specific booking based on the provided ID, and passes it to the view for editing
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)// Checks if the provided ID is null, and if so, returns a NotFound result, indicating that the requested resource cannot be found or accessed without a valid ID
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);// Asynchronously finds the booking with the specified ID in the database context, and assigns it to the variable 'booking'
            if (booking == null)
            {
                return NotFound();
            }
            var events = _context.Events
                .Select(e => new SelectListItem { Value = e.EventID.ToString(), Text = e.EventID + " (" + e.Name + ")" })
                .ToList();
            ViewData["EventID"] = new SelectList(events, "Value", "Text", booking.EventID.ToString());
            return View(booking);
        }

        /* POST: Bookings/Edit/5
         * To protect from overposting attacks, enable the specific properties you want to bind to.
         * It handles the form submission for editing an existing booking, validates the model, and if valid, it updates the booking in the database and saves changes. 
         * It also includes error handling for concurrency issues that may arise when multiple users attempt to edit the same booking simultaneously.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,EventID,CustomerName,ContactInfo,Status,CreatedAt")] Booking booking)
        {
            if (id != booking.BookingID)//Checks if the provided ID does not match the BookingID of the booking being edited, and if so, returns a NotFound result, indicating that the requested resource cannot be found or accessed with the provided ID
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try// Attempts to update the booking in the database context and save changes.
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)// Catches exceptions that occur when there is a concurrency conflict during the update operation, which can happen when multiple users attempt to edit the same booking at the same time
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;// If the venue still exists but there is a concurrency conflict, it rethrows the exception to be handled by higher-level error handling mechanisms, allowing for appropriate error responses or logging
                    }
                }
                return RedirectToAction(nameof(Index));// After successfully updating the venue, it redirects the user to the Index action, which typically displays a list of all bookings, allowing the user to see the updated information in the context of the entire list
            }
            var events = _context.Events
                .Select(e => new SelectListItem { Value = e.EventID.ToString(), Text = e.EventID + " (" + e.Name + ")" })
                .ToList();
            ViewData["EventID"] = new SelectList(events, "Value", "Text", booking.EventID.ToString());
            return View(booking);// If the model state is not valid, it repopulates the EventID dropdown list and returns the view with the current booking data, allowing the user to correct any validation errors and resubmit the form
        }

        /* GET: Bookings/Delete/5
         * It retrieves the details of a specific booking based on the provided ID, and passes it to the view for confirmation before deletion
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(m => m.BookingID == id);// Asynchronously finds the booking with the specified ID in the database context, including related event data, and assigns it to the variable 'booking'
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        /* POST: Bookings/Delete/5
         * It handles the form submission for confirming the deletion of a booking, finds the booking in the database, removes it, and saves changes. 
         * After deletion, it redirects the user to the Index action to display the updated list of bookings.
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022).Version 17.8.
         */
        [HttpPost, ActionName("Delete")]// Specifies that this action method should handle HTTP POST requests for the Delete action, and the ActionName attribute allows it to be invoked
                                        // using the "Delete" action name in the routing, even though the method name is "DeleteConfirmed"
        [ValidateAntiForgeryToken]// Validates the anti-forgery token to prevent Cross-Site Request Forgery (CSRF) attacks,
                                  // ensuring that the form submission for deletion is legitimate and comes from the expected source
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);// Asynchronously finds the booking with the specified ID in the database context, and assigns it to the variable 'booking'
            if (booking != null)
            {
                _context.Bookings.Remove(booking);// If the booking is found, it removes the booking from the database context, marking it for deletion
            }

            await _context.SaveChangesAsync();// Asynchronously saves the changes to the database, which will execute the deletion of the booking if it was found and marked for removal
            return RedirectToAction(nameof(Index));
        }

        // This private method checks if a booking with the specified ID exists in the database by querying the Bookings DbSet and
        // returns a boolean value indicating the existence of the booking
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
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