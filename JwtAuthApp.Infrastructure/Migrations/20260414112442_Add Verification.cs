using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtAuthApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_verificationCodes_Users_UserId",
                table: "verificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_verificationCodes",
                table: "verificationCodes");

            migrationBuilder.RenameTable(
                name: "verificationCodes",
                newName: "VerificationCodes");

            migrationBuilder.RenameIndex(
                name: "IX_verificationCodes_UserId",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_verificationCodes_Code",
                table: "VerificationCodes",
                newName: "IX_VerificationCodes_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationCodes",
                table: "VerificationCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCodes_Users_UserId",
                table: "VerificationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCodes_Users_UserId",
                table: "VerificationCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationCodes",
                table: "VerificationCodes");

            migrationBuilder.RenameTable(
                name: "VerificationCodes",
                newName: "verificationCodes");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_UserId",
                table: "verificationCodes",
                newName: "IX_verificationCodes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCodes_Code",
                table: "verificationCodes",
                newName: "IX_verificationCodes_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_verificationCodes",
                table: "verificationCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_verificationCodes_Users_UserId",
                table: "verificationCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
