using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiresportCalendar.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteOnRaceAndLeagueAndTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Races_Leagues_LeagueId",
                table: "Races");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_Leagues_LeagueId",
                table: "Races",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Races_Leagues_LeagueId",
                table: "Races");

            migrationBuilder.AddForeignKey(
                name: "FK_Races_Leagues_LeagueId",
                table: "Races",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }
    }
}
