using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_utilisateur_Email",
                table: "utilisateur");

            migrationBuilder.DropIndex(
                name: "IX_utilisateur_Username",
                table: "utilisateur");

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "utilisateur",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorEnabledAt",
                table: "utilisateur",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorSecret",
                table: "utilisateur",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ConversationNom",
                table: "ConversationPriver",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateur_Username_Email",
                table: "utilisateur",
                columns: new[] { "Username", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_utilisateur_Username_Email",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabledAt",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "TwoFactorSecret",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "ConversationNom",
                table: "ConversationPriver");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateur_Email",
                table: "utilisateur",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_utilisateur_Username",
                table: "utilisateur",
                column: "Username",
                unique: true);
        }
    }
}
