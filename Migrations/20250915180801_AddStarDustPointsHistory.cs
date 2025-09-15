using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CodeQuestBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStarDustPointsHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StarDustPointsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RelatedPostId = table.Column<int>(type: "integer", nullable: true),
                    RelatedCommentId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarDustPointsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StarDustPointsHistory_Comments_RelatedCommentId",
                        column: x => x.RelatedCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StarDustPointsHistory_Posts_RelatedPostId",
                        column: x => x.RelatedPostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StarDustPointsHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StarDustPointsHistory_RelatedCommentId",
                table: "StarDustPointsHistory",
                column: "RelatedCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_StarDustPointsHistory_RelatedPostId",
                table: "StarDustPointsHistory",
                column: "RelatedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_StarDustPointsHistory_UserId",
                table: "StarDustPointsHistory",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StarDustPointsHistory");
        }
    }
}
