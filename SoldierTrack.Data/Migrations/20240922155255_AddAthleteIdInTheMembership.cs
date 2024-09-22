using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoldierTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteIdInTheMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AthleteId",
                table: "Memberships",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AthleteId",
                table: "Memberships");
        }
    }
}
