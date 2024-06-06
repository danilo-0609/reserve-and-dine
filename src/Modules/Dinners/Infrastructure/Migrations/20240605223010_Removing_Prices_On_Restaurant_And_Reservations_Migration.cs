using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dinners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Removing_Prices_On_Restaurant_And_Reservations_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Refunds",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "ReservationPayments",
                schema: "dinners");

            migrationBuilder.DropColumn(
                name: "TablePrice",
                schema: "dinners",
                table: "RestaurantTables");

            migrationBuilder.DropColumn(
                name: "TablePriceCurrency",
                schema: "dinners",
                table: "RestaurantTables");

            migrationBuilder.DropColumn(
                name: "MoneyAmount",
                schema: "dinners",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "MoneyCurrency",
                schema: "dinners",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "dinners",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RefundId",
                schema: "dinners",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationPaymentId",
                schema: "dinners",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "SpecialityId",
                schema: "dinners",
                table: "Specialties",
                newName: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecialtyId",
                schema: "dinners",
                table: "Specialties",
                newName: "SpecialityId");

            migrationBuilder.AddColumn<decimal>(
                name: "TablePrice",
                schema: "dinners",
                table: "RestaurantTables",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TablePriceCurrency",
                schema: "dinners",
                table: "RestaurantTables",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MoneyAmount",
                schema: "dinners",
                table: "Reservations",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MoneyCurrency",
                schema: "dinners",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                schema: "dinners",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RefundId",
                schema: "dinners",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReservationPaymentId",
                schema: "dinners",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Refunds",
                schema: "dinners",
                columns: table => new
                {
                    RefundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoneyRefunded = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.RefundId);
                });

            migrationBuilder.CreateTable(
                name: "ReservationPayments",
                schema: "dinners",
                columns: table => new
                {
                    ReservationPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoneyPaid = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationPayments", x => x.ReservationPaymentId);
                });
        }
    }
}
