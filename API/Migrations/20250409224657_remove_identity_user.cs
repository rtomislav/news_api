using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class remove_identity_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_AspNetUsers_IdAuthorsNavigationId",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_IdAuthorsNavigationId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "IdAuthorsNavigationId",
                table: "Authors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdAuthorsNavigationId",
                table: "Authors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_IdAuthorsNavigationId",
                table: "Authors",
                column: "IdAuthorsNavigationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_AspNetUsers_IdAuthorsNavigationId",
                table: "Authors",
                column: "IdAuthorsNavigationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
