using CLDV6211_POE_PART1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace CLDV6211_POE_PART1.Models
{

    public class Event
    {
        /*Attributes for the Event class, with data annotations for validation and database mapping 
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [Key] // Specifies that this property is the primary key in the database
        public int EventID { get; set; } // Unique identifier for each event

        public string Name { get; set; } // Name of the event
        public DateTime StartDate { get; set; } // Date and time when the event starts
        public DateTime EndDate { get; set; } // Date and time when the event ends

        [ForeignKey("Venue")] // Specifies that this property is a foreign key referencing the Venue entity
        public int VenueID { get; set; } // Foreign key to the Venue where the event is held

        public Venue? Venue { get; set; } // Navigation property to the Venue entity (nullable to allow for events without an assigned venue)

        // URL to an image representing the event (populated via Azurite blob upload in Part 2)
        public string? ImageURL { get; set; }

        // Navigation property required by the EF model configuration
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}

/*References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8. 
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
 */