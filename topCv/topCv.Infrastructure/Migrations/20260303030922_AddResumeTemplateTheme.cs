using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace topCv.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeTemplateTheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemplateKey",
                table: "Resumes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "classic");

            migrationBuilder.AddColumn<string>(
                name: "ThemeJson",
                table: "Resumes",
                type: "text",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateKey",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "ThemeJson",
                table: "Resumes");
        }
    }
}
