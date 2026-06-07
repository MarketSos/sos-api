using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sos.Core.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrgTypeId",
                table: "Organizations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrgTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NameUz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OrgTypeId",
                table: "Organizations",
                column: "OrgTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgTypes_Code",
                table: "OrgTypes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_OrgTypes_OrgTypeId",
                table: "Organizations",
                column: "OrgTypeId",
                principalTable: "OrgTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_OrgTypes_OrgTypeId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "OrgTypes");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_OrgTypeId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "OrgTypeId",
                table: "Organizations");
        }
    }
}
