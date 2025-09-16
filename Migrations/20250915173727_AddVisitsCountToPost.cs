using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeQuestBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitsCountToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VisitsCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitsCount",
                table: "Posts");
        }
    }
}
