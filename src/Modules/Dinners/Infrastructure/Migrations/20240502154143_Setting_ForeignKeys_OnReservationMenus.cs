using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dinners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Setting_ForeignKeys_OnReservationMenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReservationMenus_MenuId",
                schema: "dinners",
                table: "ReservationMenus",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationMenus_Menus_MenuId",
                schema: "dinners",
                table: "ReservationMenus",
                column: "MenuId",
                principalSchema: "dinners",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationMenus_Menus_MenuId",
                schema: "dinners",
                table: "ReservationMenus");

            migrationBuilder.DropIndex(
                name: "IX_ReservationMenus_MenuId",
                schema: "dinners",
                table: "ReservationMenus");
        }
    }
}
