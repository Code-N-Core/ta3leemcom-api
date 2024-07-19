using Microsoft.EntityFrameworkCore.Migrations;
using PlatformAPI.Core.Const;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
            values: new object[] { Guid.NewGuid().ToString(), Roles.Teacher.ToString(), Roles.Teacher.ToString().ToUpper(), Guid.NewGuid().ToString() }
            );
            migrationBuilder.InsertData(
            table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
            values: new object[] { Guid.NewGuid().ToString(), Roles.Student.ToString(), Roles.Student.ToString().ToUpper(), Guid.NewGuid().ToString() }
            );
            migrationBuilder.InsertData(
            table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
            values: new object[] { Guid.NewGuid().ToString(), Roles.Parent.ToString(), Roles.Parent.ToString().ToUpper(), Guid.NewGuid().ToString() }
            );
            migrationBuilder.InsertData(
            table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
            values: new object[] { Guid.NewGuid().ToString(), Roles.Admin.ToString(), Roles.Admin.ToString().ToUpper(), Guid.NewGuid().ToString() }
            );
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles]");
        }
    }
}
