using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthorsWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToBookComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BookComments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_UserId",
                table: "BookComments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookComments_AspNetUsers_UserId",
                table: "BookComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookComments_AspNetUsers_UserId",
                table: "BookComments");

            migrationBuilder.DropIndex(
                name: "IX_BookComments_UserId",
                table: "BookComments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookComments");
        }
    }
}
