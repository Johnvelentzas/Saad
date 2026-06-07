using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saad_Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "YarnColors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "UserProcesses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "TaskDependencies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "StitchTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Processes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Patterns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Models",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Fabrics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "YarnColors");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "UserProcesses");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "TaskDependencies");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "StitchTypes");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Patterns");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Fabrics");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Customers");
        }
    }
}
