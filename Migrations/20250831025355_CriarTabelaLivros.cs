// Importa classes do Entity Framework Core necessárias para operações de migração
using Microsoft.EntityFrameworkCore.Migrations;

// Desabilita warnings de nullability para este arquivo
#nullable disable

// Define o namespace específico para as migrações da LivrariaApi
namespace LivrariaApi.Migrations
{
    // Classe partial que representa uma migração específica para criar a tabela de Livros
    // Herda da classe Migration do Entity Framework Core
    public partial class CriarTabelaLivros : Migration
    {
        // Método que define as alterações a serem aplicadas ao banco de dados
        // Executado quando a migração é aplicada (comando dotnet ef database update)
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cria uma nova tabela chamada "Livros" no banco de dados
            migrationBuilder.CreateTable(
                name: "Livros", // Nome da tabela no banco de dados
                columns: table => new // Define as colunas da tabela
                {
                    // Coluna Id - chave primária auto-incremental
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true), // Configuração específica do SQLite para auto-incremento
                    // Coluna Titulo - texto com tamanho máximo de 200 caracteres, não pode ser nulo
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    // Coluna Autor - texto com tamanho máximo de 200 caracteres, não pode ser nulo
                    Autor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    // Coluna Ano - número inteiro, não pode ser nulo
                    Ano = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table => // Define as restrições da tabela
                {
                    // Define a chave primária da tabela usando a coluna Id
                    table.PrimaryKey("PK_Livros", x => x.Id);
                });
        }

        // Método que define como reverter as alterações desta migração
        // Executado quando a migração é revertida (rollback)
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove completamente a tabela "Livros" do banco de dados
            migrationBuilder.DropTable(
                name: "Livros"); // Nome da tabela a ser removida
        } // Fim do método Down
    } // Fim da classe CriarTabelaLivros
} // Fim do namespace LivrariaApi.Migrations
