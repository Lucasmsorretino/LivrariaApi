// Arquivo: Data/DataContext.cs
// Descrição: Contexto do Entity Framework responsável pela conexão e mapeamento com o banco de dados
// Gerencia as entidades e tabelas do sistema da Livraria API
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025

// Importa o Entity Framework Core para funcionalidades de ORM (Object-Relational Mapping)
using Microsoft.EntityFrameworkCore;
// Importa os modelos de dados da aplicação
using LivrariaApi.Models;

// Define o namespace específico para classes relacionadas ao acesso a dados
namespace LivrariaApi.Data
{
    /// <summary>
    /// Classe DataContext que herda de DbContext do Entity Framework
    /// Responsável por gerenciar a conexão com o banco de dados
    /// Define as tabelas/entidades disponíveis no sistema
    /// Configura o mapeamento objeto-relacional da aplicação
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Construtor da classe DataContext
        /// Recebe as opções de configuração do DbContext como parâmetro
        /// As opções contêm informações como string de conexão e provedor do banco
        /// Chama o construtor da classe base DbContext passando as opções
        /// </summary>
        /// <param name="options">Opções de configuração do DbContext (conexão, provedor, etc.)</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            // Construtor vazio - as configurações são passadas para a classe base DbContext
            // O Entity Framework utiliza essas opções para estabelecer a conexão com o banco
        }

        /// <summary>
        /// Propriedade DbSet que representa a tabela "Livros" no banco de dados
        /// DbSet&lt;Livro&gt; permite operações CRUD (Create, Read, Update, Delete) na tabela Livros
        /// Mapeia a classe Livro para uma tabela "Livros" no banco de dados
        /// Cada instância de Livro representa uma linha na tabela
        /// </summary>
        public DbSet<Livro> Livros { get; set; } = null!;

        /// <summary>
        /// Propriedade DbSet que representa a tabela "MeusDados" no banco de dados
        /// DbSet&lt;MeusDados&gt; permite operações CRUD para os dados pessoais do estudante
        /// Mapeia a classe MeusDados para uma tabela "MeusDados" no banco de dados
        /// Utilizada para armazenar informações pessoais como nome, RU e curso
        /// </summary>
        public DbSet<MeusDados> MeusDados { get; set; } = null!;

        /// <summary>
        /// Propriedade DbSet que representa a tabela "Usuarios" no banco de dados
        /// DbSet&lt;Usuario&gt; permite operações CRUD para autenticação e autorização
        /// Mapeia a classe Usuario para uma tabela "Usuarios" no banco de dados
        /// Armazena dados de login, hash de senhas e informações de autenticação
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; } = null!;

    } // Fim da classe DataContext
} // Fim do namespace LivrariaApi.Data