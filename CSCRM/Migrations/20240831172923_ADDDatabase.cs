using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSCRM.Migrations
{
    /// <inheritdoc />
    public partial class ADDDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientNameSurname",
                table: "HotelOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientNameSurname",
                table: "HotelOrders");
        }
    }
}
