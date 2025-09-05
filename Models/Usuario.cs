// Arquivo: Models/Usuario.cs
// Descrição: Modelo para representar usuários do sistema de autenticação
// Inclui DTOs para registro, login e resposta de autenticação
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025

// Importa atributos de validação de dados do ASP.NET Core
using System.ComponentModel.DataAnnotations;

// Define o namespace para modelos de dados da aplicação LivrariaApi
namespace LivrariaApi.Models
{
    /// <summary>
    /// Classe que representa um usuário do sistema de autenticação
    /// Esta classe será mapeada para uma tabela "Usuarios" no banco de dados
    /// Armazena informações de login e dados de autenticação
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Propriedade que representa a chave primária do usuário
        /// Valor gerado automaticamente pelo banco de dados (IDENTITY)
        /// Utilizado como identificador único do usuário no sistema
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Propriedade que armazena o nome de usuário único para login
        /// Atributo Required torna o campo obrigatório com mensagem personalizada
        /// Atributo MaxLength define o tamanho máximo de 50 caracteres
        /// Deve ser único no sistema para evitar conflitos de login
        /// </summary>
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [MaxLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade que armazena o endereço de email do usuário
        /// Atributo Required torna o campo obrigatório com mensagem personalizada
        /// Atributo EmailAddress valida o formato do email
        /// Atributo MaxLength define o tamanho máximo de 100 caracteres
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Propriedade que armazena o hash criptográfico da senha do usuário
        /// NUNCA armazena a senha em texto plano por questões de segurança
        /// Utiliza HMACSHA512 para gerar hash seguro da senha
        /// Array de bytes para armazenar o resultado do hash
        /// </summary>
        [Required]
        public byte[] HashSenha { get; set; } = new byte[0];

        /// <summary>
        /// Propriedade que armazena o salt usado na criptografia da senha
        /// Salt é um valor aleatório usado junto com a senha para gerar o hash
        /// Previne ataques de rainbow table e torna cada hash único
        /// Array de bytes para armazenar o salt gerado aleatoriamente
        /// </summary>
        [Required]
        public byte[] SaltSenha { get; set; } = new byte[0];

        /// <summary>
        /// Propriedade que armazena a data e hora de criação da conta do usuário
        /// Valor padrão definido como a data/hora atual do sistema
        /// Utilizado para auditoria e controle de quando a conta foi criada
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Propriedade que indica se a conta do usuário está ativa
        /// Valor padrão true - contas são criadas ativas por padrão
        /// Permite desativar contas sem deletá-las do banco de dados
        /// Controle de acesso - usuários inativos não podem fazer login
        /// </summary>
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Propriedade que define a função/papel do usuário no sistema
        /// Atributo MaxLength define o tamanho máximo de 20 caracteres
        /// Valor padrão "user" - usuários comuns têm role de usuário
        /// Utilizado para controle de autorização e acesso a recursos
        /// </summary>
        [MaxLength(20)]
        public string Role { get; set; } = "user";
    } // Fim da classe Usuario

    /// <summary>
    /// DTO (Data Transfer Object) para registro de novos usuários
    /// Utilizado na requisição POST para criar uma nova conta
    /// Contém apenas os dados necessários para o registro
    /// </summary>
    public class UsuarioRegistroDto
    {
        /// <summary>
        /// Nome de usuário desejado para o novo usuário
        /// Atributo Required torna o campo obrigatório
        /// Atributo MinLength exige pelo menos 3 caracteres
        /// Atributo MaxLength limita a 50 caracteres
        /// </summary>
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [MinLength(3, ErrorMessage = "Nome de usuário deve ter pelo menos 3 caracteres")]
        [MaxLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de email do novo usuário
        /// Atributo Required torna o campo obrigatório
        /// Atributo EmailAddress valida o formato do email
        /// </summary>
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha em texto plano enviada pelo usuário (será criptografada)
        /// Atributo Required torna o campo obrigatório
        /// Atributo MinLength exige pelo menos 6 caracteres para segurança
        /// Será processada e transformada em hash antes de ser armazenada
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; } = string.Empty;
    } // Fim da classe UsuarioRegistroDto

    /// <summary>
    /// DTO (Data Transfer Object) para autenticação de usuários
    /// Utilizado na requisição POST para fazer login no sistema
    /// Contém apenas as credenciais necessárias para autenticação
    /// </summary>
    public class UsuarioLoginDto
    {
        /// <summary>
        /// Nome de usuário para autenticação
        /// Atributo Required torna o campo obrigatório
        /// Utilizado para localizar o usuário no banco de dados
        /// </summary>
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Senha em texto plano para autenticação
        /// Atributo Required torna o campo obrigatório
        /// Será verificada contra o hash armazenado no banco
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    } // Fim da classe UsuarioLoginDto

    /// <summary>
    /// DTO (Data Transfer Object) para resposta de autenticação bem-sucedida
    /// Retornado após login ou registro bem-sucedido
    /// Contém informações do usuário e token JWT para acesso
    /// </summary>
    public class UsuarioResponseDto
    {
        /// <summary>
        /// ID único do usuário autenticado
        /// Utilizado para identificar o usuário em requisições subsequentes
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário do usuário autenticado
        /// Informação de identificação retornada após autenticação
        /// </summary>
        public string NomeUsuario { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário autenticado
        /// Informação de contato retornada após autenticação
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Token JWT gerado para o usuário autenticado
        /// Deve ser incluído no header Authorization das próximas requisições
        /// Formato: "Bearer {token}"
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Função/papel do usuário no sistema (user, admin, etc.)
        /// Utilizado pelo cliente para controlar acesso a funcionalidades
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação da conta do usuário
        /// Informação de auditoria retornada para o cliente
        /// </summary>
        public DateTime DataCriacao { get; set; }
    } // Fim da classe UsuarioResponseDto
} // Fim do namespace LivrariaApi.Models
