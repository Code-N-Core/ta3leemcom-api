using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class updatequizandstudentanswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Quizzes_QuizId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizid",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Students_StudentId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_QuizId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_StudentId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "StudentsQuizzes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "StudentQuizid",
                table: "StudentAnswers",
                newName: "StudentQuizId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_StudentQuizid",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_StudentQuizId");

            migrationBuilder.AlterColumn<int>(
                name: "StudentQuizId",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizId",
                table: "StudentAnswers",
                column: "StudentQuizId",
                principalTable: "StudentsQuizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StudentsQuizzes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StudentQuizId",
                table: "StudentAnswers",
                newName: "StudentQuizid");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_StudentQuizId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_StudentQuizid");

            migrationBuilder.AlterColumn<int>(
                name: "StudentQuizid",
                table: "StudentAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuizId",
                table: "StudentAnswers",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentId",
                table: "StudentAnswers",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Quizzes_QuizId",
                table: "StudentAnswers",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizid",
                table: "StudentAnswers",
                column: "StudentQuizid",
                principalTable: "StudentsQuizzes",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Students_StudentId",
                table: "StudentAnswers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
