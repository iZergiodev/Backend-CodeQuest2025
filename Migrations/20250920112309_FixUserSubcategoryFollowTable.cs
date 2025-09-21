using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeQuestBackend.Migrations
{
    /// <inheritdoc />
    public partial class FixUserSubcategoryFollowTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FollowedAt",
                table: "UserSubcategoryFollows",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 11, 23, 9, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowedAt",
                table: "UserSubcategoryFollows");
        }
    }
}
