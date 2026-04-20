using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CLDV6211_POE_PART1.Models.ViewModels
{
    public class BookingFormViewModel
    {
        public int BookingID { get; set; }

        [Required(ErrorMessage = "Please select an event.")]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact info is required.")]
        public string ContactInfo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // [ValidateNever] tells the model binder to skip validation for this property.
        // SelectList is a UI-only helper — it is never submitted with the form, so without
        // this attribute ModelState.IsValid returns false on every POST, silently
        // reloading the view without saving or showing an error.
        [ValidateNever]
        public SelectList EventSelectList { get; set; } = default!;
    }
}