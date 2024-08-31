using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class SAnswerup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentId_QuizId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_StudentId_QuizId",
                table: "StudentAnswers");

            migrationBuilder.AddColumn<int>(
                name: "StudentQuizQuizId",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentQuizStudentId",
                table: "StudentAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuizId",
                table: "StudentAnswers",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentId",
                table: "StudentAnswers",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentQuizStudentId", "StudentQuizQuizId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Quizzes_QuizId",
                table: "StudentAnswers",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentQuizStudentId", "StudentQuizQuizId" },
                principalTable: "StudentsQuizzes",
                principalColumns: new[] { "StudentId", "QuizId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Students_StudentId",
                table: "StudentAnswers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Quizzes_QuizId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentQuizStudentId_StudentQuizQuizId",
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

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_StudentQuizStudentId_StudentQuizQuizId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "StudentQuizQuizId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "StudentQuizStudentId",
                table: "StudentAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentId_QuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentId", "QuizId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentId_QuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentId", "QuizId" },
                principalTable: "StudentsQuizzes",
                principalColumns: new[] { "StudentId", "QuizId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
