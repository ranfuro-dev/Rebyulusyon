using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rebyulusyon.Migrations
{
    /// <inheritdoc />
    public partial class UseAnyWeekStartDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeekStartsOnDay",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Preserve the existing Sunday or Monday preference for current accounts.
            migrationBuilder.Sql("UPDATE \"AspNetUsers\" SET \"WeekStartsOnDay\" = CASE WHEN \"WeekStartsMonday\" THEN 1 ELSE 0 END;");

            migrationBuilder.DropColumn(
                name: "WeekStartsMonday",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WeekStartsMonday",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // A rollback can preserve Monday; any other selected weekday falls back to Sunday.
            migrationBuilder.Sql("UPDATE \"AspNetUsers\" SET \"WeekStartsMonday\" = (\"WeekStartsOnDay\" = 1);");

            migrationBuilder.DropColumn(
                name: "WeekStartsOnDay",
                table: "AspNetUsers");
        }
    }
}
