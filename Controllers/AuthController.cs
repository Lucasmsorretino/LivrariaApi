// Arquivo: Controllers/AuthController.cs - Controller responsável pela autenticação e autorização
// Descrição: Controlador que gerencia operações de autenticação JWT, registro de usuários e autorização
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025 (versão com documentação linha por linha)

// Importação do contexto de dados para acesso ao banco de dados
using LivrariaApi.Data;
// Importação dos modelos de entidades (Usuario, DTOs)
using LivrariaApi.Models;
// Importação da interface de serviços de autenticação
using LivrariaApi.Services;
// Importação do atributo de autorização do ASP.NET Core
using Microsoft.AspNetCore.Authorization;
// Importação da classe base para controllers de API
using Microsoft.AspNetCore.Mvc;
// Importação do Entity Framework Core para operações assíncronas
using Microsoft.EntityFrameworkCore;

// Namespace da aplicação - organização lógica das classes
namespace LivrariaApi.Controllers
{
    // Atributo que define a rota base como /api/Auth (substitui [controller] pelo nome da classe sem "Controller")
    [Route("api/[controller]")]
    // Atributo que configura este controller como um controller de API (validações automáticas, etc.)
    [ApiController]
    // Declaração da classe AuthController que herda de ControllerBase (classe base para APIs)
    public class AuthController : ControllerBase
    {
        // Campo privado readonly para o contexto do banco de dados (injeção de dependência)
        private readonly DataContext _context;
        // Campo privado readonly para o serviço de autenticação (injeção de dependência)
        private readonly IAuthService _authService;

        // Construtor da classe que recebe dependências via injeção de dependência
        public AuthController(DataContext context, IAuthService authService)
        {
            // Inicialização do contexto de dados
            _context = context;
            // Inicialização do serviço de autenticação
            _authService = authService;
        }

        // --- MÉTODO POST - REGISTRAR NOVO USUÁRIO ---
        // Endpoint: POST /api/Auth/registrar
        // Atributo HTTP POST que define a rota específica para registro
        [HttpPost("registrar")]
        // Método assíncrono público que retorna ActionResult com UsuarioResponseDto
        public async Task<ActionResult<UsuarioResponseDto>> Registrar(UsuarioRegistroDto usuarioDto)
        {
            // Bloco try-catch para tratamento de exceções durante o registro
            try
            {
                // Verificação assíncrona se o nome de usuário já existe no banco (case-insensitive)
                if (await _context.Usuarios.AnyAsync(u => u.NomeUsuario.ToLower() == usuarioDto.NomeUsuario.ToLower()))
                {
                    // Retorna erro 400 Bad Request se o nome de usuário já existir
                    return BadRequest("Nome de usuário já existe.");
                }

                // Verificação assíncrona se o email já existe no banco (case-insensitive)
                if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == usuarioDto.Email.ToLower()))
                {
                    // Retorna erro 400 Bad Request se o email já estiver em uso
                    return BadRequest("Email já está em uso.");
                }

                // Chama o serviço de autenticação para criar hash HMACSHA512 e salt da senha
                _authService.CriarHashSenha(usuarioDto.Senha, out byte[] hashSenha, out byte[] saltSenha);

                // Criação de nova instância da entidade Usuario com dados fornecidos
                var usuario = new Usuario
                {
                    // Atribuição do nome de usuário do DTO
                    NomeUsuario = usuarioDto.NomeUsuario,
                    // Atribuição do email do DTO
                    Email = usuarioDto.Email,
                    // Atribuição do hash da senha gerado pelo serviço
                    HashSenha = hashSenha,
                    // Atribuição do salt da senha gerado pelo serviço
                    SaltSenha = saltSenha,
                    // Definição da data de criação como data/hora atual
                    DataCriacao = DateTime.Now,
                    // Definição do usuário como ativo por padrão
                    Ativo = true,
                    // Atribuição da role padrão como "user"
                    Role = "user"
                };

                // Adiciona a entidade usuário ao contexto do Entity Framework
                _context.Usuarios.Add(usuario);
                // Persiste as mudanças no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();

                // Gera token JWT para o usuário recém-criado usando o serviço de autenticação
                string token = _authService.GerarTokenJWT(usuario);

                // Criação do objeto de resposta com dados do usuário (sem informações sensíveis)
                var response = new UsuarioResponseDto
                {
                    // ID do usuário gerado pelo banco
                    Id = usuario.Id,
                    // Nome de usuário
                    NomeUsuario = usuario.NomeUsuario,
                    // Email do usuário
                    Email = usuario.Email,
                    // Token JWT para autenticação
                    Token = token,
                    // Role/perfil do usuário
                    Role = usuario.Role,
                    // Data de criação da conta
                    DataCriacao = usuario.DataCriacao
                };

                // Retorna status 201 Created com o objeto criado e location header
                return CreatedAtAction(nameof(ObterPerfil), new { id = usuario.Id }, response);
            }
            // Captura qualquer exceção que possa ocorrer durante o processo
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro ao registrar usuário: {ex.Message}");
            }
        }

        // --- MÉTODO POST - LOGIN DE USUÁRIO ---
        // Endpoint: POST /api/Auth/login
        // Atributo HTTP POST que define a rota específica para login
        [HttpPost("login")]
        // Método assíncrono público que retorna ActionResult com UsuarioResponseDto
        public async Task<ActionResult<UsuarioResponseDto>> Login(UsuarioLoginDto loginDto)
        {
            // Bloco try-catch para tratamento de exceções durante o login
            try
            {
                // Busca assíncrona do usuário no banco de dados pelo nome (case-insensitive)
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NomeUsuario.ToLower() == loginDto.NomeUsuario.ToLower());

                // Verificação se o usuário foi encontrado no banco de dados
                if (usuario == null)
                {
                    // Retorna erro 401 Unauthorized se usuário não existir (mensagem genérica por segurança)
                    return Unauthorized("Nome de usuário ou senha inválidos.");
                }

                // Verificação se a conta do usuário está ativa
                if (!usuario.Ativo)
                {
                    // Retorna erro 401 Unauthorized se a conta estiver desativada
                    return Unauthorized("Conta desativada.");
                }

                // Verifica a senha fornecida contra o hash armazenado usando HMACSHA512
                if (!_authService.VerificarSenha(loginDto.Senha, usuario.HashSenha, usuario.SaltSenha))
                {
                    // Retorna erro 401 Unauthorized se a senha estiver incorreta (mensagem genérica por segurança)
                    return Unauthorized("Nome de usuário ou senha inválidos.");
                }

                // Gera token JWT para o usuário autenticado usando o serviço de autenticação
                string token = _authService.GerarTokenJWT(usuario);

                // Criação do objeto de resposta com dados do usuário (sem informações sensíveis)
                var response = new UsuarioResponseDto
                {
                    // ID do usuário autenticado
                    Id = usuario.Id,
                    // Nome de usuário
                    NomeUsuario = usuario.NomeUsuario,
                    // Email do usuário
                    Email = usuario.Email,
                    // Token JWT para futuras requisições autenticadas
                    Token = token,
                    // Role/perfil do usuário para autorização
                    Role = usuario.Role,
                    // Data de criação da conta
                    DataCriacao = usuario.DataCriacao
                };

                // Retorna status 200 OK com os dados do usuário logado
                return Ok(response);
            }
            // Captura qualquer exceção que possa ocorrer durante o login
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro ao fazer login: {ex.Message}");
            }
        }

        // --- MÉTODO POST - LOGIN ESPECIAL COM SEUS DADOS ---
        // Endpoint: POST /api/Auth/login-lucas
        // Atributo HTTP POST que define a rota específica para login do Lucas
        [HttpPost("login-lucas")]
        // Método assíncrono público que retorna ActionResult com UsuarioResponseDto (login personalizado)
        public async Task<ActionResult<UsuarioResponseDto>> LoginLucas(UsuarioLoginDto loginDto)
        {
            // Bloco try-catch para tratamento de exceções durante o login especial
            try
            {
                // Definição das credenciais hardcoded específicas do Lucas (dados do desenvolvedor)
                string nomeCorreto = "Sorrentino";
                // Definição da senha hardcoded específica (RU do aluno)
                string senhaCorreta = "4585828";

                // Verificação se as credenciais fornecidas correspondem às credenciais hardcoded
                if (loginDto.NomeUsuario != nomeCorreto || loginDto.Senha != senhaCorreta)
                {
                    // Retorna erro 401 Unauthorized se as credenciais não corresponderem
                    return Unauthorized("Credenciais inválidas para Lucas.");
                }

                // Busca assíncrona por usuário Lucas existente no banco (por nome ou email)
                var usuarioLucas = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NomeUsuario == "Lucas" || u.Email == "lucas@livraria.com");

                // Verificação se o usuário Lucas não existe no banco de dados
                if (usuarioLucas == null)
                {
                    // Cria hash e salt para a senha hardcoded usando HMACSHA512
                    _authService.CriarHashSenha(senhaCorreta, out byte[] hashSenha, out byte[] saltSenha);

                    // Criação automática do usuário Lucas no banco de dados
                    usuarioLucas = new Usuario
                    {
                        // Nome de usuário padronizado para "Lucas"
                        NomeUsuario = "Lucas",
                        // Email padronizado para o domínio da livraria
                        Email = "lucas@livraria.com",
                        // Hash da senha gerado pelo serviço de autenticação
                        HashSenha = hashSenha,
                        // Salt da senha gerado pelo serviço de autenticação
                        SaltSenha = saltSenha,
                        // Data de criação definida como momento atual
                        DataCriacao = DateTime.Now,
                        // Conta ativa por padrão
                        Ativo = true,
                        // Role de administrador para Lucas (privilégios elevados)
                        Role = "admin"
                    };

                    // Adiciona o novo usuário ao contexto do Entity Framework
                    _context.Usuarios.Add(usuarioLucas);
                    // Persiste as mudanças no banco de dados de forma assíncrona
                    await _context.SaveChangesAsync();
                }

                // Gera token JWT para o usuário Lucas usando o serviço de autenticação
                string token = _authService.GerarTokenJWT(usuarioLucas);

                // Criação do objeto de resposta com dados do usuário Lucas
                var response = new UsuarioResponseDto
                {
                    // ID do usuário Lucas (gerado pelo banco ou existente)
                    Id = usuarioLucas.Id,
                    // Nome de usuário
                    NomeUsuario = usuarioLucas.NomeUsuario,
                    // Email do usuário
                    Email = usuarioLucas.Email,
                    // Token JWT para autenticação
                    Token = token,
                    // Role de administrador
                    Role = usuarioLucas.Role,
                    // Data de criação da conta
                    DataCriacao = usuarioLucas.DataCriacao
                };

                // Retorna status 200 OK com os dados do usuário Lucas logado
                return Ok(response);
            }
            // Captura qualquer exceção que possa ocorrer durante o login especial do Lucas
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro no login do Lucas: {ex.Message}");
            }
        }

        // --- MÉTODO GET - OBTER PERFIL DO USUÁRIO AUTENTICADO ---
        // Endpoint: GET /api/Auth/perfil
        // Atributo HTTP GET que define a rota específica para obter perfil
        [HttpGet("perfil")]
        // Atributo que exige autenticação JWT válida para acessar este endpoint
        [Authorize]
        // Método assíncrono público que retorna ActionResult com UsuarioResponseDto (perfil do usuário)
        public async Task<ActionResult<UsuarioResponseDto>> ObterPerfil()
        {
            // Bloco try-catch para tratamento de exceções durante a obtenção do perfil
            try
            {
                // Extrai o ID do usuário dos claims do token JWT (NameIdentifier)
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                // Verificação se o claim de ID do usuário existe no token
                if (userIdClaim == null)
                {
                    // Retorna erro 401 Unauthorized se o token não contém ID válido
                    return Unauthorized("Token inválido.");
                }

                // Converte o valor do claim para inteiro (ID do usuário)
                int userId = int.Parse(userIdClaim.Value);

                // Busca assíncrona do usuário no banco pelo ID extraído do token
                var usuario = await _context.Usuarios.FindAsync(userId);
                // Verificação se o usuário foi encontrado no banco de dados
                if (usuario == null)
                {
                    // Retorna erro 404 Not Found se o usuário não existir
                    return NotFound("Usuário não encontrado.");
                }

                // Criação do objeto de resposta com dados do usuário (sem token por segurança)
                var response = new UsuarioResponseDto
                {
                    // ID do usuário autenticado
                    Id = usuario.Id,
                    // Nome de usuário
                    NomeUsuario = usuario.NomeUsuario,
                    // Email do usuário
                    Email = usuario.Email,
                    // Token vazio por segurança (não expor token atual)
                    Token = "",
                    // Role/perfil do usuário
                    Role = usuario.Role,
                    // Data de criação da conta
                    DataCriacao = usuario.DataCriacao
                };

                // Retorna status 200 OK com os dados do perfil do usuário
                return Ok(response);
            }
            // Captura qualquer exceção que possa ocorrer durante a obtenção do perfil
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro ao obter perfil: {ex.Message}");
            }
        }

        // --- MÉTODO GET - LISTAR TODOS OS USUÁRIOS (APENAS ADMIN) ---
        // Endpoint: GET /api/Auth/usuarios
        // Atributo HTTP GET que define a rota específica para listar usuários
        [HttpGet("usuarios")]
        // Atributo que exige autenticação JWT e role "admin" para acessar este endpoint
        [Authorize(Roles = "admin")]
        // Método assíncrono público que retorna ActionResult com lista de UsuarioResponseDto
        public async Task<ActionResult<List<UsuarioResponseDto>>> ListarUsuarios()
        {
            // Bloco try-catch para tratamento de exceções durante a listagem de usuários
            try
            {
                // Query assíncrona que projeta dados de usuários em DTOs (sem informações sensíveis)
                var usuarios = await _context.Usuarios
                    .Select(u => new UsuarioResponseDto
                    {
                        // ID do usuário
                        Id = u.Id,
                        // Nome de usuário
                        NomeUsuario = u.NomeUsuario,
                        // Email do usuário
                        Email = u.Email,
                        // Token vazio por segurança (não expor tokens)
                        Token = "",
                        // Role/perfil do usuário
                        Role = u.Role,
                        // Data de criação da conta
                        DataCriacao = u.DataCriacao
                    })
                    // Executa a query e converte o resultado para lista
                    .ToListAsync();

                // Retorna status 200 OK com a lista de usuários
                return Ok(usuarios);
            }
            // Captura qualquer exceção que possa ocorrer durante a listagem de usuários
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro ao listar usuários: {ex.Message}");
            }
        }

        // --- MÉTODO GET POR ID (PARA CREATEDATACTION) ---
        // Atributo HTTP GET com parâmetro de rota {id} para buscar usuário específico
        [HttpGet("{id}")]
        // Atributo que exige autenticação JWT para acessar este endpoint
        [Authorize]
        // Método assíncrono público que retorna ActionResult com UsuarioResponseDto (sobrecarga por ID)
        public async Task<ActionResult<UsuarioResponseDto>> ObterPerfil(int id)
        {
            // Bloco try-catch para tratamento de exceções durante a busca por ID
            try
            {
                // Busca assíncrona do usuário no banco pelo ID fornecido na rota
                var usuario = await _context.Usuarios.FindAsync(id);
                // Verificação se o usuário foi encontrado no banco de dados
                if (usuario == null)
                {
                    // Retorna erro 404 Not Found se o usuário não existir
                    return NotFound("Usuário não encontrado.");
                }

                // Criação do objeto de resposta com dados do usuário (sem token por segurança)
                var response = new UsuarioResponseDto
                {
                    // ID do usuário encontrado
                    Id = usuario.Id,
                    // Nome de usuário
                    NomeUsuario = usuario.NomeUsuario,
                    // Email do usuário
                    Email = usuario.Email,
                    // Token vazio por segurança
                    Token = "",
                    // Role/perfil do usuário
                    Role = usuario.Role,
                    // Data de criação da conta
                    DataCriacao = usuario.DataCriacao
                };

                // Retorna status 200 OK com os dados do usuário encontrado
                return Ok(response);
            }
            // Captura qualquer exceção que possa ocorrer durante a busca
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro ao obter usuário: {ex.Message}");
            }
        }

        // --- MÉTODO DELETE - DELETAR CONTA (USUÁRIO AUTENTICADO) ---
        // Endpoint: DELETE /api/Auth/deletar-conta
        // Atributo HTTP DELETE que define a rota específica para deletar conta
        [HttpDelete("deletar-conta")]
        // Atributo que exige autenticação JWT para acessar este endpoint
        [Authorize]
        // Método assíncrono público que retorna ActionResult (deleção de conta própria)
        public async Task<ActionResult> DeletarConta()
        {
            // Bloco try-catch para tratamento de exceções durante a deleção da conta
            try
            {
                // Extrai o ID do usuário dos claims do token JWT (NameIdentifier)
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                // Verificação se o claim de ID do usuário existe no token
                if (userIdClaim == null)
                {
                    // Retorna erro 401 Unauthorized se o token não contém ID válido
                    return Unauthorized("Token inválido.");
                }

                // Converte o valor do claim para inteiro (ID do usuário)
                int userId = int.Parse(userIdClaim.Value);
                // Busca assíncrona do usuário no banco pelo ID extraído do token
                var usuario = await _context.Usuarios.FindAsync(userId);

                // Verificação se o usuário foi encontrado no banco de dados
                if (usuario == null)
                {
                    // Retorna erro 404 Not Found se o usuário não existir
                    return NotFound("Usuário não encontrado.");
                }

                // Remove o usuário do contexto do Entity Framework
                _context.Usuarios.Remove(usuario);
                // Persiste a remoção no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();

                // Retorna status 200 OK com mensagem de confirmação
                return Ok("Conta deletada com sucesso.");
            }
            // Captura qualquer exceção que possa ocorrer durante a deleção
            catch (Exception ex)
            {
                // Retorna erro 400 Bad Request com mensagem da exceção
                return BadRequest($"Erro ao deletar conta: {ex.Message}");
            }
        }
    }
}
