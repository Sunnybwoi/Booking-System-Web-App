using System.Collections.Generic;
using CLDV6211_POE_PART1.Models;

namespace CLDV6211_POE_PART1.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Venue> Venues { get; set; } = new();
        public List<Event> Events { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }
}
