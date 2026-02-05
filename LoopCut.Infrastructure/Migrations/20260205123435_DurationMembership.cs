using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoopCut.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DurationMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationInMonths",
                table: "memberships",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInMonths",
                table: "memberships");
        }
    }
}
