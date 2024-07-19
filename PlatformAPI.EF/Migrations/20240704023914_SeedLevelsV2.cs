using Microsoft.EntityFrameworkCore.Migrations;
using PlatformAPI.Core.Const;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class SeedLevelsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Levels",
                column: "Name",
                value: "الإبتدائي"
            );
            migrationBuilder.InsertData(
                table: "Levels",
                column: "Name",
                value: "الإعدادي"
            );
            migrationBuilder.InsertData(
                table: "Levels",
                column: "Name",
                value: "الثانوي"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Levels");
        }
    }
}
