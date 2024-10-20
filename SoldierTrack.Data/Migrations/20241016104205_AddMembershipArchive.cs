#nullable disable
namespace SoldierTrack.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddMembershipArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembershipArchives",
                columns: table => new
                {
                    AthleteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MembershipId = table.Column<int>(type: "int", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipArchives", x => new { x.AthleteId, x.MembershipId });
                    table.ForeignKey(
                        name: "FK_MembershipArchives_AspNetUsers_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembershipArchives_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipArchives_MembershipId",
                table: "MembershipArchives",
                column: "MembershipId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipArchives");
        }
    }
}
