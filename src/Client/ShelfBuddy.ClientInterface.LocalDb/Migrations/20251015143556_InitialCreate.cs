using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShelfBuddy.ClientInterface.LocalDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsSynced = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsSynced = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsSynced = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalInventoryLocalProduct",
                columns: table => new
                {
                    LocalInventoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalInventoryLocalProduct", x => new { x.LocalInventoryId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_LocalInventoryLocalProduct_Inventories_LocalInventoryId",
                        column: x => x.LocalInventoryId,
                        principalTable: "Inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocalInventoryLocalProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_IsSynced",
                table: "Inventories",
                column: "IsSynced");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_UserId",
                table: "Inventories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalInventoryLocalProduct_ProductsId",
                table: "LocalInventoryLocalProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_IsSynced",
                table: "ProductCategories",
                column: "IsSynced");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Name",
                table: "ProductCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsSynced",
                table: "Products",
                column: "IsSynced");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalInventoryLocalProduct");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
