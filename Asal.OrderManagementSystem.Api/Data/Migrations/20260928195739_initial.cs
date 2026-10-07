using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Asal.OrderManagementSystem.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.CheckConstraint("CK_Products_Price_Positive", "[Price] > 0");
                    table.CheckConstraint("CK_Products_StockQuantity_NotNegatives", "[StockQuantity] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.CheckConstraint("CK_OrderItem_Price_MustBePositive", "[UnitPrice]>0");
                    table.CheckConstraint("CK_OrderItem_Quantity_MustBePositive", "[Quantity]>0");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "ahmad@example.com", "Ahmad Ali", "0599123456" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "omar@example.com", "Omar Khalil", "0598765432" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "sara@example.com", "Sara Hassan", "0599555444" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "Price", "SKU", "StockQuantity" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Laptop", 899.99m, "LAP-001", 15 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Wireless Mouse", 29.99m, "MOU-001", 50 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Mechanical Keyboard", 79.99m, "KEY-001", 25 },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "USB-C Cable", 14.99m, "USB-001", 100 },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Monitor", 249.99m, "MON-001", 8 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "Status" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "Completed" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("11111111-1111-1111-1111-111111111111"), "Completed" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("22222222-2222-2222-2222-222222222222"), "Completed" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("22222222-2222-2222-2222-222222222222"), "Pending" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), "Completed" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), 1, 899.99m },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("55555555-5555-5555-5555-555555555555"), 2, 29.99m },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("66666666-6666-6666-6666-666666666666"), 1, 79.99m },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("77777777-7777-7777-7777-777777777777"), 3, 14.99m },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("44444444-4444-4444-4444-444444444444"), 2, 899.99m },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("88888888-8888-8888-8888-888888888888"), 1, 249.99m },
                    { new Guid("10000000-0000-0000-0000-000000000007"), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("55555555-5555-5555-5555-555555555555"), 2, 29.99m },
                    { new Guid("10000000-0000-0000-0000-000000000008"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("66666666-6666-6666-6666-666666666666"), 2, 79.99m },
                    { new Guid("10000000-0000-0000-0000-000000000009"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("77777777-7777-7777-7777-777777777777"), 5, 14.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
