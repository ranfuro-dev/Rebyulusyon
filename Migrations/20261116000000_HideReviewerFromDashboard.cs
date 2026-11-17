using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rebyulusyon.Migrations;

// Add a per-user dashboard visibility flag without changing reviewers or study history.
public partial class HideReviewerFromDashboard : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsHiddenFromDashboard",
            table: "ReviewerStudies",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsHiddenFromDashboard",
            table: "ReviewerStudies");
    }
}
