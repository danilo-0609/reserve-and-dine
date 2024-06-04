using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Correcting_UserOutboxMessage_Schema_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UsersOutboxMessages",
                schema: "dinners",
                newName: "UsersOutboxMessages",
                newSchema: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dinners");

            migrationBuilder.RenameTable(
                name: "UsersOutboxMessages",
                schema: "users",
                newName: "UsersOutboxMessages",
                newSchema: "dinners");
        }
    }
}
