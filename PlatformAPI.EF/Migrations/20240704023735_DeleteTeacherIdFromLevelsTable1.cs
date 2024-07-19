using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTeacherIdFromLevelsTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Teachers_TeacherId",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_TeacherId",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Levels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Levels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Levels_TeacherId",
                table: "Levels",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Teachers_TeacherId",
                table: "Levels",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
