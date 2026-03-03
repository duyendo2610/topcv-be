using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace topCv.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeTemplateCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TemplateKey",
                table: "Resumes",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "simple",
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldDefaultValue: "classic");

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateVariantId",
                table: "Resumes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ThemePresetId",
                table: "Resumes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResumeTemplateVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    VariantKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LayoutKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeTemplateVariants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResumeThemePresets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThemeKey = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ThemeJson = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeThemePresets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResumeTemplateVariantThemes",
                columns: table => new
                {
                    VariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThemePresetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeTemplateVariantThemes", x => new { x.VariantId, x.ThemePresetId });
                    table.ForeignKey(
                        name: "FK_ResumeTemplateVariantThemes_ResumeTemplateVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ResumeTemplateVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumeTemplateVariantThemes_ResumeThemePresets_ThemePresetId",
                        column: x => x.ThemePresetId,
                        principalTable: "ResumeThemePresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_TemplateVariantId",
                table: "Resumes",
                column: "TemplateVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_ThemePresetId",
                table: "Resumes",
                column: "ThemePresetId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTemplateVariants_TemplateKey",
                table: "ResumeTemplateVariants",
                column: "TemplateKey");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTemplateVariants_VariantKey",
                table: "ResumeTemplateVariants",
                column: "VariantKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumeTemplateVariantThemes_ThemePresetId",
                table: "ResumeTemplateVariantThemes",
                column: "ThemePresetId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeThemePresets_TemplateKey",
                table: "ResumeThemePresets",
                column: "TemplateKey");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeThemePresets_ThemeKey",
                table: "ResumeThemePresets",
                column: "ThemeKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_ResumeTemplateVariants_TemplateVariantId",
                table: "Resumes",
                column: "TemplateVariantId",
                principalTable: "ResumeTemplateVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_ResumeThemePresets_ThemePresetId",
                table: "Resumes",
                column: "ThemePresetId",
                principalTable: "ResumeThemePresets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_ResumeTemplateVariants_TemplateVariantId",
                table: "Resumes");

            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_ResumeThemePresets_ThemePresetId",
                table: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeTemplateVariantThemes");

            migrationBuilder.DropTable(
                name: "ResumeTemplateVariants");

            migrationBuilder.DropTable(
                name: "ResumeThemePresets");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_TemplateVariantId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_ThemePresetId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "TemplateVariantId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "ThemePresetId",
                table: "Resumes");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateKey",
                table: "Resumes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "classic",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldDefaultValue: "simple");
        }
    }
}
