using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelancePay.Migrations
{
    /// <inheritdoc />
    public partial class addtransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedByClient",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VerifiedByFreelance",
                table: "Payments");

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Recipient = table.Column<string>(type: "text", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    TransferCode = table.Column<string>(type: "text", nullable: false),
                    PaystackTransferId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.AddColumn<bool>(
                name: "VerifiedByClient",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VerifiedByFreelance",
                table: "Payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
