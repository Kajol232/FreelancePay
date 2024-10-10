using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelancePay.Migrations
{
    /// <inheritdoc />
    public partial class addbool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isExtended",
                table: "Invoices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isExtended",
                table: "Invoices");
        }
    }
}
