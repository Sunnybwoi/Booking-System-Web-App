using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDV6211_POE_PART1.Models
{
    public class Booking
    {
        /*Attributes for the Booking class, with data annotations for validation and database mapping 
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [Key] // Specifies that this property is the primary key in the database
        public int BookingID { get; set; } // Unique identifier for each booking

        [ForeignKey("Event")] // Specifies that this property is a foreign key referencing the Event entity
        public int EventID { get; set; }  // FK to Event 

        public Event? Event { get; set; }  // Navigation property to the Event entity (nullable to allow for bookings without an assigned event)

        public string CustomerName { get; set; } // Name of the customer making the booking
        public string ContactInfo { get; set; } // Contact information for the customer (e.g., email or phone number)
        public string Status { get; set; } = "Pending"; // Status of the booking (e.g., Pending, Confirmed, Cancelled), defaulting to "Pending"
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp for when the booking was created, defaulting to the current date and time
    }
}

/*References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8. 
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
 */