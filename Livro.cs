// Importa atributos de validação de dados do ASP.NET Core
using System.ComponentModel.DataAnnotations;

// Define o namespace da aplicação LivrariaApi
namespace LivrariaApi
{
    // Declaração da classe Livro que representa o modelo de dados de um livro
    // Esta classe será mapeada para uma tabela no banco de dados pelo Entity Framework
    public class Livro
    {
        // Propriedade que representa a chave primária do livro
        // O Entity Framework automaticamente reconhece "Id" como chave primária
        public int Id { get; set; }

        // Propriedade que armazena o título do livro
        // Atributo MaxLength define o tamanho máximo de 200 caracteres para o campo
        [MaxLength(200)]
        // Propriedade com valor padrão como string vazia para evitar valores null
        public string Titulo { get; set; } = string.Empty;

        // Propriedade que armazena o nome do autor do livro
        // Atributo MaxLength define o tamanho máximo de 200 caracteres para o campo
        [MaxLength(200)]
        // Propriedade com valor padrão como string vazia para evitar valores null
        public string Autor { get; set; } = string.Empty;

        // Propriedade que armazena o ano de publicação do livro
        // Tipo int para representar anos como números inteiros
        public int Ano { get; set; }
    } // Fim da classe Livro
} // Fim do namespace LivrariaApi
