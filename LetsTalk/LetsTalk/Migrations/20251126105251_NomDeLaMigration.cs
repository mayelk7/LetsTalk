using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Migrations
{
    /// <inheritdoc />
    public partial class NomDeLaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConversationPriver",
                columns: table => new
                {
                    ConversationPriverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationPriver", x => x.ConversationPriverId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "fichier",
                columns: table => new
                {
                    FichierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fichier", x => x.FichierId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "utilisateur",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilPicture = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Actif = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Type2Fa = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateur", x => x.UtilisateurId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "membreMP",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_membreMP", x => new { x.UtilisateurId, x.ConversationId });
                    table.ForeignKey(
                        name: "FK_membreMP_ConversationPriver_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "ConversationPriver",
                        principalColumn: "ConversationPriverId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_membreMP_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messageLu",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    Lu = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageLu", x => new { x.UtilisateurId, x.MessageType, x.MessageId });
                    table.ForeignKey(
                        name: "FK_messageLu_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messagePriver",
                columns: table => new
                {
                    MessagePriverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Contenu = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateEnvoi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Epingle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    ConversationPriverId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messagePriver", x => x.MessagePriverId);
                    table.ForeignKey(
                        name: "FK_messagePriver_ConversationPriver_ConversationPriverId",
                        column: x => x.ConversationPriverId,
                        principalTable: "ConversationPriver",
                        principalColumn: "ConversationPriverId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messagePriver_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    Lu = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_notification_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "server",
                columns: table => new
                {
                    ServerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.ServerId);
                    table.ForeignKey(
                        name: "FK_server_utilisateur_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "canaux",
                columns: table => new
                {
                    CanauxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_canaux", x => x.CanauxId);
                    table.ForeignKey(
                        name: "FK_canaux_server_ServerId",
                        column: x => x.ServerId,
                        principalTable: "server",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    ServerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_role_server_ServerId",
                        column: x => x.ServerId,
                        principalTable: "server",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messageCanal",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Contenu = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateEnvoi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Epingle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    CanalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageCanal", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_messageCanal_canaux_CanalId",
                        column: x => x.CanalId,
                        principalTable: "canaux",
                        principalColumn: "CanauxId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messageCanal_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Membre",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    ServerId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membre", x => new { x.UtilisateurId, x.ServerId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Membre_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Membre_server_ServerId",
                        column: x => x.ServerId,
                        principalTable: "server",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Membre_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "UtilisateurId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_canaux_ServerId",
                table: "canaux",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Membre_RoleId",
                table: "Membre",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Membre_ServerId",
                table: "Membre",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_membreMP_ConversationId",
                table: "membreMP",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_messageCanal_CanalId_DateEnvoi",
                table: "messageCanal",
                columns: new[] { "CanalId", "DateEnvoi" });

            migrationBuilder.CreateIndex(
                name: "IX_messageCanal_UtilisateurId",
                table: "messageCanal",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_messagePriver_ConversationPriverId_DateEnvoi",
                table: "messagePriver",
                columns: new[] { "ConversationPriverId", "DateEnvoi" });

            migrationBuilder.CreateIndex(
                name: "IX_messagePriver_UtilisateurId",
                table: "messagePriver",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_UtilisateurId",
                table: "notification",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_role_ServerId",
                table: "role",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_server_OwnerId",
                table: "server",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fichier");

            migrationBuilder.DropTable(
                name: "Membre");

            migrationBuilder.DropTable(
                name: "membreMP");

            migrationBuilder.DropTable(
                name: "messageCanal");

            migrationBuilder.DropTable(
                name: "messageLu");

            migrationBuilder.DropTable(
                name: "messagePriver");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "canaux");

            migrationBuilder.DropTable(
                name: "ConversationPriver");

            migrationBuilder.DropTable(
                name: "server");

            migrationBuilder.DropTable(
                name: "utilisateur");
        }
    }
}
