using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CLDV6211_POE_PART1.Models;

namespace CLDV6211_POE_PART1.Data
{
    public class CLDV6211_DbContext : DbContext
    {
        public CLDV6211_DbContext(DbContextOptions<CLDV6211_DbContext> options)
            : base(options)
        {
        }

        /* Generated with assistance from Anthropic (2026) Claude [AI assistant].
         * Prompt: Prompt: 'How do I set up a DbContext with DbSet properties for models
         * in Entity Framework Core in C#', 22 March 2026. */
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }

        /* Optional: Configure relationships & auto-increment IDs
         * Generated with assistance from Anthropic (2026) Claude [AI assistant].
         * Prompt: 'How do I configure auto-increment primary keys and foreign key
         * relationships with cascade delete restrictions using OnModelCreating
         * in Entity Framework Core', 22 March 2026. */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure IDs auto-generate
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Event>()
                .Property(e => e.EventID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Venue>()
                .Property(v => v.VenueID)
                .ValueGeneratedOnAdd();

            // Optional: Set up FK relationships explicitly
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueID)
                .OnDelete(DeleteBehavior.Restrict); 
            
            // Prevent cascade delete if booking exists
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

/* References
 * Anthropic (2026) Claude [AI assistant]. Available at: https://claude.ai (Accessed: 22 March 2026). 
 * Prompt used: 'How do I set up a DbContext with DbSet properties for models
 * in Entity Framework Core in C#'.
 * 
 * Anthropic (2026) Claude [AI assistant]. Available at: https://claude.ai (Accessed: 22 March 2026). 
 * Prompt used: 'How do I configure auto-increment primary keys and foreign key
 * relationships with cascade delete restrictions using OnModelCreating
 * in Entity Framework Core'.
 */