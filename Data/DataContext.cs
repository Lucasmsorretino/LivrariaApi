// Data/DataContext.cs
// Importa o Entity Framework Core para funcionalidades de ORM (Object-Relational Mapping)
using Microsoft.EntityFrameworkCore;

// Define o namespace específico para classes relacionadas ao acesso a dados
namespace LivrariaApi.Data // Este é o namespace que estava faltando
{
    // Declaração da classe DataContext que herda de DbContext
    // DbContext é a classe base do Entity Framework para interação com banco de dados
    public class DataContext : DbContext // Herda da classe DbContext do Entity Framework
    {
        // Construtor da classe que recebe as opções de configuração do DbContext
        // O parâmetro options contém configurações como string de conexão e provedor do banco
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            // Construtor vazio - as configurações são passadas para a classe base DbContext
        }

        // Propriedade DbSet que representa a tabela "Livros" no banco de dados
        // DbSet<Livro> permite operações CRUD (Create, Read, Update, Delete) na tabela Livros
        // Mapeia a classe Livro para uma tabela "Livros" no banco de dados
        public DbSet<Livro> Livros { get; set; } = null!;

    } // Fim da classe DataContext
} // Fim do namespace LivrariaApi.Data