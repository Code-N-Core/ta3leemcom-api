using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class addIsSubscribedColumnInTeacherTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubscribed",
                table: "Teachers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubscribed",
                table: "Teachers");
        }
    }
}
