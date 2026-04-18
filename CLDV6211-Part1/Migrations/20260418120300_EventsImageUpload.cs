using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLDV6211_POE_PART1.Migrations
{
    /// <inheritdoc />
    public partial class EventsImageUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Events");
        }
    }
}
