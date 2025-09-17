using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CodeQuestBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StarDustPointsHistory_Comments_RelatedCommentId",
                table: "StarDustPointsHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StarDustPointsHistory_Posts_RelatedPostId",
                table: "StarDustPointsHistory");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    RelatedPostId = table.Column<int>(type: "integer", nullable: true),
                    RelatedCommentId = table.Column<int>(type: "integer", nullable: true),
                    RelatedUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Comments_RelatedCommentId",
                        column: x => x.RelatedCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Posts_RelatedPostId",
                        column: x => x.RelatedPostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_RelatedUserId",
                        column: x => x.RelatedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedCommentId",
                table: "Notifications",
                column: "RelatedCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedPostId",
                table: "Notifications",
                column: "RelatedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedUserId",
                table: "Notifications",
                column: "RelatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StarDustPointsHistory_Comments_RelatedCommentId",
                table: "StarDustPointsHistory",
                column: "RelatedCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StarDustPointsHistory_Posts_RelatedPostId",
                table: "StarDustPointsHistory",
                column: "RelatedPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StarDustPointsHistory_Comments_RelatedCommentId",
                table: "StarDustPointsHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StarDustPointsHistory_Posts_RelatedPostId",
                table: "StarDustPointsHistory");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_StarDustPointsHistory_Comments_RelatedCommentId",
                table: "StarDustPointsHistory",
                column: "RelatedCommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StarDustPointsHistory_Posts_RelatedPostId",
                table: "StarDustPointsHistory",
                column: "RelatedPostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
