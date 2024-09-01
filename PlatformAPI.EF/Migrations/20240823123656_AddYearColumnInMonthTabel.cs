using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddYearColumnInMonthTabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Months",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Months");
        }
    }
}
