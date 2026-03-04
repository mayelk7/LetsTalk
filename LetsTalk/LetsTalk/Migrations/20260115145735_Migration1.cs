using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Membre_role_RoleId",
                table: "Membre");

            migrationBuilder.DropForeignKey(
                name: "FK_Membre_server_ServerId",
                table: "Membre");

            migrationBuilder.DropForeignKey(
                name: "FK_Membre_utilisateur_UtilisateurId",
                table: "Membre");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Membre",
                table: "Membre");

            migrationBuilder.RenameTable(
                name: "Membre",
                newName: "membre");

            migrationBuilder.RenameIndex(
                name: "IX_Membre_ServerId",
                table: "membre",
                newName: "IX_membre_ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_Membre_RoleId",
                table: "membre",
                newName: "IX_membre_RoleId");

            migrationBuilder.AlterColumn<string>(
                name: "Type2Fa",
                table: "utilisateur",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_membre",
                table: "membre",
                columns: new[] { "UtilisateurId", "ServerId", "RoleId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_membre_role_RoleId",
                table: "membre",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_membre_server_ServerId",
                table: "membre",
                column: "ServerId",
                principalTable: "server",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_membre_utilisateur_UtilisateurId",
                table: "membre",
                column: "UtilisateurId",
                principalTable: "utilisateur",
                principalColumn: "UtilisateurId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_membre_role_RoleId",
                table: "membre");

            migrationBuilder.DropForeignKey(
                name: "FK_membre_server_ServerId",
                table: "membre");

            migrationBuilder.DropForeignKey(
                name: "FK_membre_utilisateur_UtilisateurId",
                table: "membre");

            migrationBuilder.DropIndex(
                name: "IX_utilisateur_Email",
                table: "utilisateur");

            migrationBuilder.DropIndex(
                name: "IX_utilisateur_Username",
                table: "utilisateur");

            migrationBuilder.DropPrimaryKey(
                name: "PK_membre",
                table: "membre");

            migrationBuilder.RenameTable(
                name: "membre",
                newName: "Membre");

            migrationBuilder.RenameIndex(
                name: "IX_membre_ServerId",
                table: "Membre",
                newName: "IX_Membre_ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_membre_RoleId",
                table: "Membre",
                newName: "IX_Membre_RoleId");

            migrationBuilder.UpdateData(
                table: "utilisateur",
                keyColumn: "Type2Fa",
                keyValue: null,
                column: "Type2Fa",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type2Fa",
                table: "utilisateur",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Membre",
                table: "Membre",
                columns: new[] { "UtilisateurId", "ServerId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Membre_role_RoleId",
                table: "Membre",
                column: "RoleId",
                principalTable: "role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Membre_server_ServerId",
                table: "Membre",
                column: "ServerId",
                principalTable: "server",
                principalColumn: "ServerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Membre_utilisateur_UtilisateurId",
                table: "Membre",
                column: "UtilisateurId",
                principalTable: "utilisateur",
                principalColumn: "UtilisateurId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
