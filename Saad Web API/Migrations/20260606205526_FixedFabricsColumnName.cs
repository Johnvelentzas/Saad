using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saad_Web_API.Migrations
{
    /// <inheritdoc />
    public partial class FixedFabricsColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YarnColorName",
                table: "Fabrics",
                newName: "FabricName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FabricName",
                table: "Fabrics",
                newName: "YarnColorName");
        }
    }
}
