using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sos.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    NameUz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NameUzKiril = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    NameUz = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NameUzKiril = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: true),
                    FixedPrice = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    DiscountPct = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MinQuantity = table.Column<int>(type: "integer", nullable: false),
                    MaxQuantity = table.Column<int>(type: "integer", nullable: true),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    NameUz = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NameUzKiril = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,3)", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(10,3)", nullable: true),
                    CostPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ExpirationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastOutcomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skus_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Skus_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MeasurementUnits",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsDeleted", "NameEn", "NameRu", "NameUz", "NameUzKiril", "OrganizationId", "UpdatedAt", "UpdatedBy", "Version" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), "dona", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3919), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "Piece", "Штука", "Dona", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3933), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000002"), "kg", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3934), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "kg", "Кг", "Kilogramm", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3935), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000003"), "g", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3966), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "g", "Грамм", "Gramm", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3967), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000004"), "l", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3969), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "l", "Литр", "Litr", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3970), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000005"), "ml", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3971), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "ml", "Мл", "Millilitr", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(3972), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000006"), "m", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4024), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "m", "Метр", "Metr", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4026), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000007"), "m2", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4027), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "m²", "Кв.м", "Kv. metr", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4028), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000008"), "box", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4032), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "box", "Коробка", "Quti", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4034), new TimeSpan(0, 0, 0, 0, 0)), null, 0 },
                    { new Guid("00000000-0000-0000-0001-000000000009"), "pack", new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4035), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, null, false, "pack", "Пачка", "Paket", null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTimeOffset(new DateTime(2026, 6, 6, 8, 36, 23, 485, DateTimeKind.Unspecified).AddTicks(4036), new TimeSpan(0, 0, 0, 0, 0)), null, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUnits_Code",
                table: "MeasurementUnits",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceRules_ProductId_StoreId_IsActive",
                table: "PriceRules",
                columns: new[] { "ProductId", "StoreId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Skus_MeasurementUnitId",
                table: "Skus",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Skus_ProductId",
                table: "Skus",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Skus_SerialNumber",
                table: "Skus",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_ProductId_StoreId",
                table: "StockItems",
                columns: new[] { "ProductId", "StoreId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceRules");

            migrationBuilder.DropTable(
                name: "Skus");

            migrationBuilder.DropTable(
                name: "StockItems");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
