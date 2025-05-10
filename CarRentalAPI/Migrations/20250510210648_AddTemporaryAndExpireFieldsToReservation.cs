using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTemporaryAndExpireFieldsToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporary",
                table: "Reservations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsTemporary",
                table: "Reservations");
        }
    }
}
