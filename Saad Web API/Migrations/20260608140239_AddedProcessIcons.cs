using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saad_Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddedProcessIcons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Processes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconText",
                table: "Processes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "IconText",
                table: "Processes");
        }
    }
}
