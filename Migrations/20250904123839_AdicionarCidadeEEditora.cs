using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LivrariaApi.Migrations
{
    public partial class AdicionarCidadeEEditora : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Livros",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Editora",
                table: "Livros",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Livros");

            migrationBuilder.DropColumn(
                name: "Editora",
                table: "Livros");
        }
    }
}
