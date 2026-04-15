using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CLDV6211_POE_PART1.Models.ViewModels
{
    public class BookingFormViewModel
    {
        public int BookingID { get; set; }
        public int EventID { get; set; }
        public string CustomerName { get; set; }
        public string ContactInfo { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public SelectList EventSelectList { get; set; }
    }
}
