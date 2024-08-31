using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class SAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswer_Chooses_ChosenOptionId",
                table: "StudentAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswer_Questions_QuestionId",
                table: "StudentAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswer_StudentsQuizzes_StudentId_QuizId",
                table: "StudentAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAnswer",
                table: "StudentAnswer");

            migrationBuilder.RenameTable(
                name: "StudentAnswer",
                newName: "StudentAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswer_StudentId_QuizId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_StudentId_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswer_QuestionId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswer_ChosenOptionId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_ChosenOptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAnswers",
                table: "StudentAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Chooses_ChosenOptionId",
                table: "StudentAnswers",
                column: "ChosenOptionId",
                principalTable: "Chooses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentId_QuizId",
                table: "StudentAnswers",
                columns: new[] { "StudentId", "QuizId" },
                principalTable: "StudentsQuizzes",
                principalColumns: new[] { "StudentId", "QuizId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Chooses_ChosenOptionId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Questions_QuestionId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_StudentsQuizzes_StudentId_QuizId",
                table: "StudentAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAnswers",
                table: "StudentAnswers");

            migrationBuilder.RenameTable(
                name: "StudentAnswers",
                newName: "StudentAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_StudentId_QuizId",
                table: "StudentAnswer",
                newName: "IX_StudentAnswer_StudentId_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_QuestionId",
                table: "StudentAnswer",
                newName: "IX_StudentAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_ChosenOptionId",
                table: "StudentAnswer",
                newName: "IX_StudentAnswer_ChosenOptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAnswer",
                table: "StudentAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswer_Chooses_ChosenOptionId",
                table: "StudentAnswer",
                column: "ChosenOptionId",
                principalTable: "Chooses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswer_Questions_QuestionId",
                table: "StudentAnswer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswer_StudentsQuizzes_StudentId_QuizId",
                table: "StudentAnswer",
                columns: new[] { "StudentId", "QuizId" },
                principalTable: "StudentsQuizzes",
                principalColumns: new[] { "StudentId", "QuizId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
