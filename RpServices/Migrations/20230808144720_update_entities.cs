using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpServices.Migrations
{
    /// <inheritdoc />
    public partial class update_entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_UserCards_UserId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_UserId",
                table: "Cards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId",
                table: "Cards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_UserCards_UserId",
                table: "Cards",
                column: "UserId",
                principalTable: "UserCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
