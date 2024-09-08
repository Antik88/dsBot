using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dsbot.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayListTracksRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayListId",
                table: "Tracks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_PlayListId",
                table: "Tracks",
                column: "PlayListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_PlayLists_PlayListId",
                table: "Tracks",
                column: "PlayListId",
                principalTable: "PlayLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_PlayLists_PlayListId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_PlayListId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "PlayListId",
                table: "Tracks");
        }
    }
}
