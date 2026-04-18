using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLDV6211_POE_PART1.Migrations
{
    /// <inheritdoc />
    public partial class BlobStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenueID",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VenueID",
                table: "Bookings",
                column: "VenueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Venues_VenueID",
                table: "Bookings",
                column: "VenueID",
                principalTable: "Venues",
                principalColumn: "VenueID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Venues_VenueID",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_VenueID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VenueID",
                table: "Bookings");
        }
    }
}
