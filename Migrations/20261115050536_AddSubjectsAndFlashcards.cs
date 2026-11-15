using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rebyulusyon.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectsAndFlashcards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviewers_OwnerId",
                table: "Reviewers");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Reviewers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Flashcards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Answer = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ReviewerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flashcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flashcards_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_OwnerId_SubjectId",
                table: "Reviewers",
                columns: new[] { "OwnerId", "SubjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_SubjectId",
                table: "Reviewers",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_ReviewerId_SortOrder",
                table: "Flashcards",
                columns: new[] { "ReviewerId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_OwnerId",
                table: "Subjects",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_Subjects_SubjectId",
                table: "Reviewers",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_Subjects_SubjectId",
                table: "Reviewers");

            migrationBuilder.DropTable(
                name: "Flashcards");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Reviewers_OwnerId_SubjectId",
                table: "Reviewers");

            migrationBuilder.DropIndex(
                name: "IX_Reviewers_SubjectId",
                table: "Reviewers");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Reviewers");

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_OwnerId",
                table: "Reviewers",
                column: "OwnerId");
        }
    }
}
