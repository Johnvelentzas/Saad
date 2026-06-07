using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saad_Web_API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionWorkflowTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttributeValues");

            migrationBuilder.DropTable(
                name: "PatternAreas");

            migrationBuilder.DropTable(
                name: "TaskAtributes");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AFabricId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BFabricId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CFabricId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DFabricId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DropOffApt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstYarnColorId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FoamType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasBoltTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCheckTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCustomPatternTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasCutTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasDropOffApt",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasEmbroideryTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFoamTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasGelTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMultipleFabrics",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPickUpApt",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRipTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSewTask",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasTestTryApt",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PatternId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickUpApt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RipAction",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondYarnColorId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StitchTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TestTryApt",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Fabrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: false),
                    YarnColorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StitchTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: false),
                    StitchTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StitchTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarnColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: false),
                    YarnColorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarnColors", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fabrics");

            migrationBuilder.DropTable(
                name: "StitchTypes");

            migrationBuilder.DropTable(
                name: "YarnColors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AFabricId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BFabricId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CFabricId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DFabricId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DropOffApt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FirstYarnColorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FoamType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasBoltTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasCheckTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasCustomPatternTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasCutTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasDropOffApt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasEmbroideryTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasFoamTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasGelTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasMultipleFabrics",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasPickUpApt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasRipTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasSewTask",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HasTestTryApt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PatternId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PickUpApt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RipAction",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SecondYarnColorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StitchTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TestTryApt",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "AttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatternAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatternId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatternAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskAtributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAtributes", x => x.Id);
                });
        }
    }
}
