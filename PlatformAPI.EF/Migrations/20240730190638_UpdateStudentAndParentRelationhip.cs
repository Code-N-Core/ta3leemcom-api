using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentAndParentRelationhip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentsAbsences_Students_StudentCode",
                table: "StudentsAbsences");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsMonths_Students_StudentCode",
                table: "StudentsMonths");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsQuizzes_Students_StudentCode",
                table: "StudentsQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsMonths",
                table: "StudentsMonths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsAbsences",
                table: "StudentsAbsences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "StudentsQuizzes");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "StudentsMonths");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "StudentsAbsences");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudentsMonths",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudentsAbsences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes",
                columns: new[] { "StudentId", "QuizId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsMonths",
                table: "StudentsMonths",
                columns: new[] { "StudentId", "MonthId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsAbsences",
                table: "StudentsAbsences",
                columns: new[] { "StudentId", "DayId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ParentId",
                table: "Students",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students",
                column: "ParentId",
                principalTable: "Parents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsAbsences_Students_StudentId",
                table: "StudentsAbsences",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsMonths_Students_StudentId",
                table: "StudentsMonths",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsQuizzes_Students_StudentId",
                table: "StudentsQuizzes",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Parents_ParentId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsAbsences_Students_StudentId",
                table: "StudentsAbsences");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsMonths_Students_StudentId",
                table: "StudentsMonths");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsQuizzes_Students_StudentId",
                table: "StudentsQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsMonths",
                table: "StudentsMonths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsAbsences",
                table: "StudentsAbsences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ParentId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentsQuizzes");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentsMonths");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentsAbsences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "StudentsQuizzes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "StudentsMonths",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "StudentsAbsences",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes",
                columns: new[] { "StudentCode", "QuizId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsMonths",
                table: "StudentsMonths",
                columns: new[] { "StudentCode", "MonthId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsAbsences",
                table: "StudentsAbsences",
                columns: new[] { "StudentCode", "DayId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsAbsences_Students_StudentCode",
                table: "StudentsAbsences",
                column: "StudentCode",
                principalTable: "Students",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsMonths_Students_StudentCode",
                table: "StudentsMonths",
                column: "StudentCode",
                principalTable: "Students",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsQuizzes_Students_StudentCode",
                table: "StudentsQuizzes",
                column: "StudentCode",
                principalTable: "Students",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
