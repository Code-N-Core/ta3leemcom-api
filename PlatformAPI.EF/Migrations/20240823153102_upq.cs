using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class upq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "StudentQuizQuizId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "StudentQuizStudentId",
                table: "StudentAnswers",
                newName: "StudentQuizid");

            migrationBuilder.AlterColumn<int>(
                name: "QuizId",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentsQuizzes_StudentId",
                table: "StudentsQuizzes",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizid",
                table: "StudentAnswers",
                column: "StudentQuizid");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizid",
                table: "StudentAnswers",
                column: "StudentQuizid",
                principalTable: "StudentsQuizzes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizid",
                table: "StudentAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_StudentsQuizzes_StudentId",
                table: "StudentsQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_StudentQuizid",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "id",
                table: "StudentsQuizzes");

            migrationBuilder.RenameColumn(
                name: "StudentQuizid",
                table: "StudentAnswers",
                newName: "StudentQuizStudentId");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<int>(
                name: "QuizId",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<int>(
                name: "StudentQuizQuizId",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsQuizzes",
                table: "StudentsQuizzes",
                columns: new[] { "StudentId", "QuizId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentQuizStudentId", "StudentQuizQuizId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentQuizStudentId", "StudentQuizQuizId" },
                principalTable: "StudentsQuizzes",
                principalColumns: new[] { "StudentId", "QuizId" });
        }
    }
}
