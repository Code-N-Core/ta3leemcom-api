using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformAPI.EF.Migrations
{
    /// <inheritdoc />
    public partial class updateQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "TimeExist",
                table: "Quizzes");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Quizzes",
                newName: "StartDate");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "StudentsQuizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAttend",
                table: "StudentsQuizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AnswerForm",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QuestionForm",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudentsQuizzes");

            migrationBuilder.DropColumn(
                name: "IsAttend",
                table: "StudentsQuizzes");

            migrationBuilder.DropColumn(
                name: "AnswerForm",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuestionForm",
                table: "Quizzes");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Quizzes",
                newName: "Date");

            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TimeExist",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
