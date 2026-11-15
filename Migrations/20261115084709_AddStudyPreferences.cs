using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rebyulusyon.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyGoal",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 20);

            migrationBuilder.AddColumn<bool>(
                name: "WeekStartsMonday",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyGoal",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WeekStartsMonday",
                table: "AspNetUsers");
        }
    }
}
