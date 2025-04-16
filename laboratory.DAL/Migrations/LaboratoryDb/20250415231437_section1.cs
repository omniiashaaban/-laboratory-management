using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace laboratory.DAL.Migrations.LaboratoryDb
{
    /// <inheritdoc />
    public partial class section1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Groups_GroupId",
                table: "Sections");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Sections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttendanceJson",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Groups_GroupId",
                table: "Sections",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_Groups_GroupId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "AttendanceJson",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Sections");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Sections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_Groups_GroupId",
                table: "Sections",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
