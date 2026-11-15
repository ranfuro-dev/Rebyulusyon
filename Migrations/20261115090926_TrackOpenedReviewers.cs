using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rebyulusyon.Migrations
{
    /// <inheritdoc />
    public partial class TrackOpenedReviewers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewerStudies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ReviewerId = table.Column<int>(type: "integer", nullable: false),
                    LastOpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewerStudies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewerStudies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewerStudies_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerStudies_ReviewerId",
                table: "ReviewerStudies",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerStudies_UserId_LastOpenedAtUtc",
                table: "ReviewerStudies",
                columns: new[] { "UserId", "LastOpenedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerStudies_UserId_ReviewerId",
                table: "ReviewerStudies",
                columns: new[] { "UserId", "ReviewerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewerStudies");
        }
    }
}
