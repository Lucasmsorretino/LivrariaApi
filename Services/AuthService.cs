// Arquivo: Services/AuthService.cs
// Descrição: Serviço responsável pela autenticação, geração de tokens JWT e criptografia de senhas
// Implementa hashing com HMACSHA512 e geração de tokens JWT seguros
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025

// Importa modelos de dados da aplicação
using LivrariaApi.Models;
// Importa classes para trabalhar com tokens de segurança
using Microsoft.IdentityModel.Tokens;
// Importa classes para manipulação de JWT (JSON Web Tokens)
using System.IdentityModel.Tokens.Jwt;
// Importa classes para trabalhar com claims de segurança
using System.Security.Claims;
// Importa classes para criptografia e hashing
using System.Security.Cryptography;
// Importa classes para codificação de texto
using System.Text;

// Define o namespace para serviços da aplicação
namespace LivrariaApi.Services
{
    /// <summary>
    /// Interface que define o contrato para o serviço de autenticação
    /// Especifica os métodos disponíveis para autenticação e criptografia
    /// Permite injeção de dependência e facilita testes unitários
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Método assíncrono para registrar um novo usuário no sistema
        /// </summary>
        /// <param name="usuarioDto">DTO contendo dados do usuário a ser registrado</param>
        /// <returns>DTO de resposta com dados do usuário e token, ou null se falhar</returns>
        Task<UsuarioResponseDto?> RegistrarUsuario(UsuarioRegistroDto usuarioDto);

        /// <summary>
        /// Método assíncrono para autenticar um usuário existente
        /// </summary>
        /// <param name="loginDto">DTO contendo credenciais de login</param>
        /// <returns>DTO de resposta com dados do usuário e token, ou null se falhar</returns>
        Task<UsuarioResponseDto?> AutenticarUsuario(UsuarioLoginDto loginDto);

        /// <summary>
        /// Método para gerar token JWT para um usuário autenticado
        /// </summary>
        /// <param name="usuario">Objeto usuário para o qual gerar o token</param>
        /// <returns>String contendo o token JWT</returns>
        string GerarTokenJWT(Usuario usuario);

        /// <summary>
        /// Método para criar hash criptográfico de uma senha usando HMACSHA512
        /// </summary>
        /// <param name="senha">Senha em texto plano a ser criptografada</param>
        /// <param name="hashSenha">Parâmetro de saída contendo o hash da senha</param>
        /// <param name="saltSenha">Parâmetro de saída contendo o salt utilizado</param>
        void CriarHashSenha(string senha, out byte[] hashSenha, out byte[] saltSenha);

        /// <summary>
        /// Método para verificar se uma senha está correta comparando com o hash armazenado
        /// </summary>
        /// <param name="senha">Senha em texto plano a ser verificada</param>
        /// <param name="hashSenha">Hash armazenado da senha correta</param>
        /// <param name="saltSenha">Salt usado na criação do hash</param>
        /// <returns>True se a senha estiver correta, False caso contrário</returns>
        bool VerificarSenha(string senha, byte[] hashSenha, byte[] saltSenha);
    } // Fim da interface IAuthService

    /// <summary>
    /// Implementação do serviço de autenticação
    /// Contém a lógica para autenticação JWT e criptografia de senhas
    /// Utiliza HMACSHA512 para hashing seguro das senhas
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Campo privado para armazenar a configuração da aplicação
        /// Utilizado para acessar chaves secretas e configurações JWT
        /// Injetado via Dependency Injection no construtor
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Construtor da classe AuthService
        /// Recebe a configuração da aplicação via injeção de dependência
        /// </summary>
        /// <param name="configuration">Interface de configuração da aplicação</param>
        public AuthService(IConfiguration configuration)
        {
            // Atribui a configuração recebida ao campo privado da classe
            _configuration = configuration;
        }

        /// <summary>
        /// Método para registrar um novo usuário no sistema
        /// NOTA: Este método está parcialmente implementado - a persistência deve ser feita no controller
        /// </summary>
        /// <param name="usuarioDto">DTO contendo dados do usuário a ser registrado</param>
        /// <returns>DTO de resposta com dados do usuário e token, ou null se falhar</returns>
        public Task<UsuarioResponseDto?> RegistrarUsuario(UsuarioRegistroDto usuarioDto)
        {
            try
            {
                // Criar hash e salt da senha utilizando HMACSHA512
                CriarHashSenha(usuarioDto.Senha, out byte[] hashSenha, out byte[] saltSenha);

                // Criar novo objeto usuário com os dados fornecidos
                var usuario = new Usuario
                {
                    NomeUsuario = usuarioDto.NomeUsuario,    // Nome de usuário fornecido
                    Email = usuarioDto.Email,                // Email fornecido
                    HashSenha = hashSenha,                   // Hash criptográfico da senha
                    SaltSenha = saltSenha,                   // Salt usado na criptografia
                    DataCriacao = DateTime.Now,              // Data atual de criação
                    Ativo = true,                            // Usuário ativo por padrão
                    Role = "user"                            // Role padrão de usuário comum
                };

                // Gerar token JWT para o usuário recém-criado
                string token = GerarTokenJWT(usuario);

                // Retornar DTO de resposta com dados do usuário e token
                return Task.FromResult<UsuarioResponseDto?>(new UsuarioResponseDto
                {
                    Id = usuario.Id,                         // ID do usuário (será 0 até ser salvo no BD)
                    NomeUsuario = usuario.NomeUsuario,       // Nome de usuário
                    Email = usuario.Email,                   // Email do usuário
                    Token = token,                           // Token JWT gerado
                    Role = usuario.Role,                     // Role do usuário
                    DataCriacao = usuario.DataCriacao        // Data de criação
                });
            }
            catch (Exception)
            {
                // Em caso de erro, retornar null
                return Task.FromResult<UsuarioResponseDto?>(null);
            }
        }

        /// <summary>
        /// Método para autenticar usuário existente
        /// NOTA: Este método não está implementado aqui - deve ser implementado no controller
        /// O controller tem acesso ao contexto do banco de dados para buscar o usuário
        /// </summary>
        /// <param name="loginDto">DTO contendo credenciais de login</param>
        /// <returns>Lança NotImplementedException</returns>
        public Task<UsuarioResponseDto?> AutenticarUsuario(UsuarioLoginDto loginDto)
        {
            // Este método será implementado no controller com acesso ao banco
            throw new NotImplementedException("Este método deve ser implementado no controller");
        }

        /// <summary>
        /// Método para gerar token JWT (JSON Web Token) para um usuário
        /// O token contém claims (informações) do usuário e é assinado digitalmente
        /// </summary>
        /// <param name="usuario">Objeto usuário para o qual gerar o token</param>
        /// <returns>String contendo o token JWT</returns>
        public string GerarTokenJWT(Usuario usuario)
        {
            // Obter chave secreta da configuração ou usar chave padrão
            var chaveSecreta = _configuration["JwtSettings:SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%";
            // Converter a chave para array de bytes usando codificação ASCII
            var key = Encoding.ASCII.GetBytes(chaveSecreta);

            // Criar lista de claims (informações) que serão incluídas no token
            var claims = new List<Claim>
            {
                // Claim com ID do usuário - utilizado para identificar o usuário
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                // Claim com nome do usuário - utilizado para exibição
                new Claim(ClaimTypes.Name, usuario.NomeUsuario),
                // Claim com email do usuário - utilizado para identificação adicional
                new Claim(ClaimTypes.Email, usuario.Email),
                // Claim com role do usuário - utilizado para autorização
                new Claim(ClaimTypes.Role, usuario.Role),
                // Claim com timestamp de emissão do token (issued at time)
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Criar descritor do token com todas as configurações
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),                    // Identidade com claims do usuário
                Expires = DateTime.UtcNow.AddHours(24),                  // Token expira em 24 horas
                Issuer = _configuration["JwtSettings:Issuer"] ?? "LivrariaApi",      // Emissor do token
                Audience = _configuration["JwtSettings:Audience"] ?? "LivrariaApi",  // Audiência do token
                SigningCredentials = new SigningCredentials(             // Credenciais de assinatura
                    new SymmetricSecurityKey(key),                       // Chave simétrica para assinatura
                    SecurityAlgorithms.HmacSha512Signature               // Algoritmo HMAC-SHA512
                )
            };

            // Criar manipulador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            // Criar o token usando o descritor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            // Converter o token para string e retornar
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Método para criar hash criptográfico de uma senha usando HMACSHA512
        /// Gera um salt aleatório e calcula o hash da senha com esse salt
        /// </summary>
        /// <param name="senha">Senha em texto plano a ser criptografada</param>
        /// <param name="hashSenha">Parâmetro de saída - array de bytes com o hash da senha</param>
        /// <param name="saltSenha">Parâmetro de saída - array de bytes com o salt utilizado</param>
        public void CriarHashSenha(string senha, out byte[] hashSenha, out byte[] saltSenha)
        {
            // Criar instância do HMACSHA512 - gera chave aleatória automaticamente
            using (var hmac = new HMACSHA512())
            {
                // A chave gerada pelo HMAC serve como salt (valor aleatório)
                saltSenha = hmac.Key;
                // Calcular hash da senha usando o salt como chave do HMAC
                hashSenha = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
            }
        }

        /// <summary>
        /// Método para verificar se uma senha está correta
        /// Recalcula o hash usando a senha fornecida e o salt armazenado
        /// Compara com o hash armazenado para verificar se são iguais
        /// </summary>
        /// <param name="senha">Senha em texto plano a ser verificada</param>
        /// <param name="hashSenha">Hash armazenado da senha correta</param>
        /// <param name="saltSenha">Salt usado na criação do hash original</param>
        /// <returns>True se a senha estiver correta, False caso contrário</returns>
        public bool VerificarSenha(string senha, byte[] hashSenha, byte[] saltSenha)
        {
            // Criar instância do HMACSHA512 usando o salt armazenado como chave
            using (var hmac = new HMACSHA512(saltSenha))
            {
                // Calcular hash da senha fornecida usando o mesmo salt
                var hashComputado = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
                
                // Comparar os hashes byte a byte para verificar se são idênticos
                for (int i = 0; i < hashComputado.Length; i++)
                {
                    // Se algum byte for diferente, a senha está incorreta
                    if (hashComputado[i] != hashSenha[i])
                        return false;
                }
                
                // Se todos os bytes forem iguais, a senha está correta
                return true;
            }
        }
    } // Fim da classe AuthService
} // Fim do namespace LivrariaApi.Services
