using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetsTalk.Migrations
{
    public partial class alter_table_ConversationPriver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "conversationNom",
                table: "ConversationPriver",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "conversationNom",
                table: "ConversationPriver");
        }
    }
}
