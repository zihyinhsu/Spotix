using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Spotix.API.Migrations
{
    /// <inheritdoc />
    public partial class updateAreaandTicketModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatSelection",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "Reciever",
                table: "Tickets",
                newName: "RecieverId");

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "Areas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "25935620-161c-4c2f-8936-fe88436ad02c", "25935620-161c-4c2f-8936-fe88436ad02c", "User", "USER" },
                    { "d7bece5e-cba2-4f5a-a158-2f56919bd43d", "d7bece5e-cba2-4f5a-a158-2f56919bd43d", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "25935620-161c-4c2f-8936-fe88436ad02c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d7bece5e-cba2-4f5a-a158-2f56919bd43d");

            migrationBuilder.RenameColumn(
                name: "RecieverId",
                table: "Tickets",
                newName: "Reciever");

            migrationBuilder.AddColumn<bool>(
                name: "SeatSelection",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "DisplayOrder",
                table: "Areas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
