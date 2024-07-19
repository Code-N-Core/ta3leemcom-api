using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class SeedLevelsYears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
                "SELECT N'الأول الإبتدائي', Id" +
                " FROM Levels" +
                " WHERE Name=N'الإبتدائي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الثاني الإبتدائي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإبتدائي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الثالث الإبتدائي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإبتدائي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الرابع الإبتدائي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإبتدائي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الخامس الإبتدائي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإبتدائي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'السادس الإبتدائي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإبتدائي'");

            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الأول الإعدادي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإعدادي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الثاني الإعدادي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإعدادي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الثالث الإعدادي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الإعدادي'");

            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الأول الثانوي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الثانوي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الثاني الثانوي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الثانوي'");
            migrationBuilder.Sql("INSERT INTO LevelYears (Name, LevelId)" +
               "SELECT N'الثالث الثانوي', Id" +
               " FROM Levels" +
               " WHERE Name=N'الثانوي'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM LevelYears");
        }
    }
}
