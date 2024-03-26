using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dinners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Dinners_Module_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dinners");

            migrationBuilder.CreateTable(
                name: "DinnersOutboxMessages",
                schema: "dinners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OcurredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinnersOutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountTerms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PriceCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsVegetarian = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryChefName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasAlcohol = table.Column<bool>(type: "bit", nullable: false),
                    MainCourse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SideDishes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Appetizers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Beverages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desserts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sauces = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Condiments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Coffee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Open = table.Column<TimeSpan>(type: "time", nullable: false),
                    Close = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                schema: "dinners",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                schema: "dinners",
                columns: table => new
                {
                    RefundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoneyRefunded = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    PayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MoneyPaid = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationPayments", x => x.ReservationPaymentId);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "dinners",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservedTable = table.Column<int>(type: "int", nullable: false),
                    MoneyAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservationStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReservationEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReservationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfAttendees = table.Column<int>(type: "int", nullable: false),
                    ReservationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservationPaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefundId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantRatingId",
                columns: table => new
                {
                    Value = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantRatingId", x => x.Value);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfTables = table.Column<int>(type: "int", nullable: false),
                    AvailableTableStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestaurantScheduleStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Whatsapp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Facebook = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instagram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Twitter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TikTok = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.RestaurantId);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "dinners",
                columns: table => new
                {
                    MenuReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.MenuReviewId);
                });

            migrationBuilder.CreateTable(
                name: "DaysAvailable",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayAvailable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaysAvailable", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_DaysAvailable_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dinners",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ingredient = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_Ingredients_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dinners",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuConsumers",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuConsumers", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_MenuConsumers_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dinners",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuImagesUrl",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuImagesUrl", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_MenuImagesUrl_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dinners",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenusReviews",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenusReviews", x => new { x.MenuId, x.MenuReviewId });
                    table.ForeignKey(
                        name: "FK_MenusReviews_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dinners",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "dinners",
                columns: table => new
                {
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.MenuId);
                    table.ForeignKey(
                        name: "FK_Tags_Menus_MenuId",
                        column: x => x.MenuId,
                        principalSchema: "dinners",
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservationMenus",
                schema: "dinners",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationMenus", x => new { x.ReservationId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_ReservationMenus_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "dinners",
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chefs",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Chef = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chefs", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_Chefs_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DaysOfOperation",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfOperation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaysOfOperation", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_DaysOfOperation_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantTables",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumberOfTable = table.Column<int>(type: "int", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    IsOccuppied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantTables", x => x.NumberOfTable);
                    table.ForeignKey(
                        name: "FK_RestaurantTables_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantAdministrations",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdministratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdministratorTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantAdministrations", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_RestaurantAdministrations_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantClients",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfVisits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantClients", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_RestaurantClients_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantImagesUrl",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantImagesUrl", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_RestaurantImagesUrl_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantRatings",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantRatings", x => new { x.RestaurantId, x.RatingId });
                    table.ForeignKey(
                        name: "FK_RestaurantRatings_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                schema: "dinners",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Speciality = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.RestaurantId);
                    table.ForeignKey(
                        name: "FK_Specialties_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalSchema: "dinners",
                        principalTable: "Restaurants",
                        principalColumn: "RestaurantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservedHour",
                schema: "dinners",
                columns: table => new
                {
                    ReservationDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartReservationTimeRange = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndReservationTimeRange = table.Column<TimeSpan>(type: "time", nullable: false),
                    NumberOfTable = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedHour", x => new { x.NumberOfTable});
                    table.ForeignKey(
                        name: "FK_ReservedHour_ReservedHours_RestaurantTableNumberOfTable_RestaurantTableNumber",
                        column: x =>  x.NumberOfTable,
                        principalSchema: "dinners",
                        principalTable: "RestaurantTables",
                        principalColumn: "NumberOfTable",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chefs",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "DaysAvailable",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "DaysOfOperation",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "DinnersOutboxMessages",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Ingredients",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "MenuConsumers",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "MenuImagesUrl",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "MenusReviews",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Ratings",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Refunds",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "ReservationMenus",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "ReservationPayments",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "ReservedHour",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "RestaurantAdministrations",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "RestaurantClients",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "RestaurantImagesUrl",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "RestaurantRatings",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Reviews",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Specialties",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "MenuReviewId");

            migrationBuilder.DropTable(
                name: "MenuId");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "ReservedHours",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "RestaurantRatingId");

            migrationBuilder.DropTable(
                name: "Menus",
                schema: "dinners");

            migrationBuilder.DropTable(
                name: "Restaurants",
                schema: "dinners");
        }
    }
}
