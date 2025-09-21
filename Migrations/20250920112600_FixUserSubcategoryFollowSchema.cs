using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeQuestBackend.Migrations
{
    /// <inheritdoc />
    public partial class FixUserSubcategoryFollowSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mark the initial migration as applied if not already
            migrationBuilder.Sql(@"
                INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"") 
                VALUES ('20250920071601_InitialCreate', '9.0.1') 
                ON CONFLICT (""MigrationId"") DO NOTHING");

            // Add FollowedAt column if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'UserSubcategoryFollows' 
                        AND column_name = 'FollowedAt'
                    ) THEN
                        ALTER TABLE ""UserSubcategoryFollows"" 
                        ADD COLUMN ""FollowedAt"" timestamp with time zone NOT NULL DEFAULT NOW();
                    END IF;
                END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the FollowedAt column
            migrationBuilder.Sql(@"
                ALTER TABLE ""UserSubcategoryFollows"" 
                DROP COLUMN IF EXISTS ""FollowedAt""");
        }
    }
}
