using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace topCv.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobSubmittedAtUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "Jobs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittedAtUtc",
                table: "Jobs");
        }
    }
}
