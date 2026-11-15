using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rebyulusyon.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FlashcardId = table.Column<int>(type: "integer", nullable: false),
                    GotIt = table.Column<bool>(type: "boolean", nullable: false),
                    StudiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyActivities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyActivities_Flashcards_FlashcardId",
                        column: x => x.FlashcardId,
                        principalTable: "Flashcards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyActivities_FlashcardId",
                table: "StudyActivities",
                column: "FlashcardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyActivities_UserId_FlashcardId_StudiedAtUtc",
                table: "StudyActivities",
                columns: new[] { "UserId", "FlashcardId", "StudiedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_StudyActivities_UserId_StudiedAtUtc",
                table: "StudyActivities",
                columns: new[] { "UserId", "StudiedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyActivities");
        }
    }
}
