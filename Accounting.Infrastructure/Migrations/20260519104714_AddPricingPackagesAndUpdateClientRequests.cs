using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingPackagesAndUpdateClientRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_clientRequests",
                table: "clientRequests");

            migrationBuilder.RenameTable(
                name: "clientRequests",
                newName: "ClientRequests");

            migrationBuilder.RenameIndex(
                name: "IX_clientRequests_Status",
                table: "ClientRequests",
                newName: "IX_ClientRequests_Status");

            migrationBuilder.RenameIndex(
                name: "IX_clientRequests_RequestType",
                table: "ClientRequests",
                newName: "IX_ClientRequests_RequestType");

            migrationBuilder.AddColumn<Guid>(
                name: "PricingPackageId",
                table: "ClientRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientRequests",
                table: "ClientRequests",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PricingPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Badge = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    PriceLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PeriodLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsRecommended = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Features = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPackages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientRequests_CreatedAtUtc",
                table: "ClientRequests",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRequests_PricingPackageId",
                table: "ClientRequests",
                column: "PricingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRequests_ServiceId",
                table: "ClientRequests",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingPackages_IsActive",
                table: "PricingPackages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PricingPackages_SortOrder",
                table: "PricingPackages",
                column: "SortOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_PricingPackages_PricingPackageId",
                table: "ClientRequests",
                column: "PricingPackageId",
                principalTable: "PricingPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientRequests_Services_ServiceId",
                table: "ClientRequests",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_PricingPackages_PricingPackageId",
                table: "ClientRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientRequests_Services_ServiceId",
                table: "ClientRequests");

            migrationBuilder.DropTable(
                name: "PricingPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientRequests",
                table: "ClientRequests");

            migrationBuilder.DropIndex(
                name: "IX_ClientRequests_CreatedAtUtc",
                table: "ClientRequests");

            migrationBuilder.DropIndex(
                name: "IX_ClientRequests_PricingPackageId",
                table: "ClientRequests");

            migrationBuilder.DropIndex(
                name: "IX_ClientRequests_ServiceId",
                table: "ClientRequests");

            migrationBuilder.DropColumn(
                name: "PricingPackageId",
                table: "ClientRequests");

            migrationBuilder.RenameTable(
                name: "ClientRequests",
                newName: "clientRequests");

            migrationBuilder.RenameIndex(
                name: "IX_ClientRequests_Status",
                table: "clientRequests",
                newName: "IX_clientRequests_Status");

            migrationBuilder.RenameIndex(
                name: "IX_ClientRequests_RequestType",
                table: "clientRequests",
                newName: "IX_clientRequests_RequestType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_clientRequests",
                table: "clientRequests",
                column: "Id");
        }
    }
}
