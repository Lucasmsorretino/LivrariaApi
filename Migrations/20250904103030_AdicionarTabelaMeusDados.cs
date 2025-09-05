using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LivrariaApi.Migrations
{
    public partial class AdicionarTabelaMeusDados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeusDados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RU = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Curso = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeusDados", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeusDados");
        }
    }
}
