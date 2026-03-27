using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLDV6211_POE_PART1.Migrations
{
    /// <inheritdoc />
    public partial class FixContextAndKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Event_EventID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Venue_VenueID",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Venue",
                table: "Venue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Booking",
                table: "Booking");

            migrationBuilder.RenameTable(
                name: "Venue",
                newName: "Venues");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "Booking",
                newName: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_Event_VenueID",
                table: "Events",
                newName: "IX_Events_VenueID");

            migrationBuilder.RenameIndex(
                name: "IX_Booking_EventID",
                table: "Bookings",
                newName: "IX_Bookings_EventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Venues",
                table: "Venues",
                column: "VenueID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "EventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Events_EventID",
                table: "Bookings",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueID",
                table: "Events",
                column: "VenueID",
                principalTable: "Venues",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Events_EventID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueID",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Venues",
                table: "Venues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "Venues",
                newName: "Venue");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Booking");

            migrationBuilder.RenameIndex(
                name: "IX_Events_VenueID",
                table: "Event",
                newName: "IX_Event_VenueID");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_EventID",
                table: "Booking",
                newName: "IX_Booking_EventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Venue",
                table: "Venue",
                column: "VenueID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "EventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Booking",
                table: "Booking",
                column: "BookingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Event_EventID",
                table: "Booking",
                column: "EventID",
                principalTable: "Event",
                principalColumn: "EventID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Venue_VenueID",
                table: "Event",
                column: "VenueID",
                principalTable: "Venue",
                principalColumn: "VenueID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
