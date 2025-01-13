using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotix.API.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketandOrderfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Tickets",
                newName: "TicketNumber");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "Tickets",
                newName: "RowNumber");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Orders",
                newName: "OrderNumber");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketNumber",
                table: "Tickets",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "RowNumber",
                table: "Tickets",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                table: "Orders",
                newName: "Number");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
