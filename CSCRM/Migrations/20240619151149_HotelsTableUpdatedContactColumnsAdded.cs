using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSCRM.Migrations
{
    /// <inheritdoc />
    public partial class HotelsTableUpdatedContactColumnsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Hotels");
        }
    }
}
