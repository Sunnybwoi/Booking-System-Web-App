namespace CLDV6211_POE_PART1.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Venue
    {
        /*Attributes for the Venue class, with data annotations for validation and database mapping 
         * Code completion assisted by Visual Studio IntelliSense
         * (Microsoft Corporation, 2022). Version 17.8.
         */
        [Key]
        public int VenueID { get; set; } // Primary key

        public string Name { get; set; } // Name of the venue
        public string Location { get; set; } // Physical address or description of the venue's location
        public int Capacity { get; set; } // Maximum number of people the venue can accommodate
        public string ImageURL { get; set; } // URL to an image representing the venue
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp for when the venue was created

        // Navigation property required by the DbContext mapping
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}

/*References
 * Microsoft Corporation (2022) Visual Studio IntelliSense [Software]. Version 17.8. 
 * Available at: https://visualstudio.microsoft.com (Accessed: 22 March 2026).
*/
