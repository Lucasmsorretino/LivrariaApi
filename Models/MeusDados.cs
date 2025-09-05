// Arquivo: Models/MeusDados.cs
// Descrição: Modelo para representar os dados pessoais do estudante no sistema
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025

// Importa atributos de validação de dados do ASP.NET Core
using System.ComponentModel.DataAnnotations;

// Define o namespace para modelos de dados da aplicação LivrariaApi
namespace LivrariaApi.Models
{
    /// <summary>
    /// Classe que representa os dados pessoais do estudante
    /// Esta classe será mapeada para uma tabela no banco de dados pelo Entity Framework
    /// </summary>
    public class MeusDados
    {
        /// <summary>
        /// Propriedade que representa a chave primária do registro de dados pessoais
        /// O Entity Framework automaticamente reconhece "Id" como chave primária
        /// Valor gerado automaticamente pelo banco de dados (IDENTITY)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Propriedade que armazena o nome completo do estudante
        /// Atributo MaxLength define o tamanho máximo de 200 caracteres para o campo
        /// Atributo Required torna o campo obrigatório com mensagem de erro personalizada
        /// Valor padrão como string vazia para evitar valores null
        /// </summary>
        [MaxLength(200)]
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade que armazena o Registro Único (RU) do estudante
        /// Atributo MaxLength define o tamanho máximo de 20 caracteres para o campo
        /// Atributo Required torna o campo obrigatório com mensagem de erro personalizada
        /// Valor padrão como string vazia para evitar valores null
        /// </summary>
        [MaxLength(20)]
        [Required(ErrorMessage = "RU é obrigatório")]
        public string RU { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade que armazena o nome do curso do estudante
        /// Atributo MaxLength define o tamanho máximo de 300 caracteres para o campo
        /// Atributo Required torna o campo obrigatório com mensagem de erro personalizada
        /// Valor padrão como string vazia para evitar valores null
        /// </summary>
        [MaxLength(300)]
        [Required(ErrorMessage = "Curso é obrigatório")]
        public string Curso { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade que armazena a data e hora de criação do registro
        /// Valor padrão definido como a data/hora atual do sistema
        /// Utilizado para auditoria e controle de quando os dados foram inseridos
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    } // Fim da classe MeusDados
} // Fim do namespace LivrariaApi.Models
