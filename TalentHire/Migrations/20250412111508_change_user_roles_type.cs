using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentHire.Migrations
{
    /// <inheritdoc />
    public partial class change_user_roles_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""Role"" DROP DEFAULT;");
           migrationBuilder.Sql(@"
    ALTER TABLE ""Users""
    ALTER COLUMN ""Role"" TYPE integer USING
    CASE
        WHEN ""Role"" = 'Admin' THEN 1
        WHEN ""Role"" = 'User' THEN 0
        WHEN ""Role"" = 'Employer' THEN 2
        ELSE NULL
    END;
");
    migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""Role"" SET DEFAULT 0; -- for example, 1 = User");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
