// Arquivo: Controllers/MeusDadosController.cs
// Descrição: Controller específico para gerenciar dados pessoais do estudante
// Fornece endpoints para CRUD completo dos dados pessoais com validações
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025

// Importa o contexto de dados para acesso ao banco de dados
using LivrariaApi.Data;
// Importa os modelos de dados da aplicação
using LivrariaApi.Models;
// Importa funcionalidades do ASP.NET Core MVC para controllers
using Microsoft.AspNetCore.Mvc;
// Importa funcionalidades do Entity Framework para operações assíncronas
using Microsoft.EntityFrameworkCore;
// Importa coleções genéricas como List<T>
using System.Collections.Generic;
// Importa suporte para programação assíncrona
using System.Threading.Tasks;

// Define o namespace que organiza as classes relacionadas aos controllers
namespace LivrariaApi.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar operações CRUD dos dados pessoais do estudante
    /// Fornece endpoints separados para dados hardcoded e dados dinâmicos no banco
    /// Atributo Route define a rota base como "api/MeusDados"
    /// Atributo ApiController habilita funcionalidades automáticas de validação
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MeusDadosController : ControllerBase
    {
        /// <summary>
        /// Campo privado readonly que armazena a instância do contexto do banco de dados
        /// Utilizado para realizar operações CRUD na tabela MeusDados
        /// Injetado via Dependency Injection no construtor
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Objeto anônimo contendo dados pessoais hardcoded (fixos no código)
        /// Utilizado pelo endpoint /hardcoded para retornar dados estáticos
        /// Contém informações atualizadas do estudante
        /// </summary>
        private readonly object _meusDadosHardcoded = new
        {
            Nome = "Lucas Martins Sorrentino",           // Nome completo do estudante
            RU = "4585828",                              // Registro Único do estudante
            Curso = "Análise e Desenvolvimento de Sistemas"  // Nome do curso atualizado
        };

        /// <summary>
        /// Construtor da classe MeusDadosController
        /// Recebe uma instância do DataContext via injeção de dependência
        /// O ASP.NET Core injeta automaticamente a dependência através do sistema de DI
        /// </summary>
        /// <param name="context">Contexto do Entity Framework para acesso ao banco</param>
        public MeusDadosController(DataContext context)
        {
            // Atribui o contexto recebido ao campo privado da classe
            _context = context;
        }

       
        
        [HttpGet]
        public async Task<ActionResult<List<MeusDados>>> GetTodosMeusDados()
        {
            try
            {
                // Executa consulta assíncrona para buscar todos os registros da tabela MeusDados
                var dados = await _context.MeusDados.ToListAsync();
                // Retorna status HTTP 200 OK com a lista de dados encontrados
                return Ok(dados);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status HTTP 400 Bad Request com mensagem do erro
                return BadRequest($"Erro ao buscar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Método GET assíncrono que busca dados pessoais específicos por ID
        /// Endpoint: GET /api/MeusDados/{id}
        /// Atributo HttpGet com template de rota que captura o ID da URL
        /// </summary>
        /// <param name="id">ID do registro a ser buscado</param>
        /// <returns>ActionResult contendo dados específicos ou erro se não encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MeusDados>> GetMeusDadosPorId(int id)
        {
            try
            {
                // Executa busca assíncrona por chave primária (ID) usando FindAsync
                var dados = await _context.MeusDados.FindAsync(id);
                
                // Verifica se o registro foi encontrado no banco de dados
                if (dados == null)
                    // Retorna status HTTP 404 Not Found se o registro não existir
                    return NotFound("Dados não encontrados.");

                // Retorna status HTTP 200 OK com os dados encontrados
                return Ok(dados);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status HTTP 400 Bad Request com mensagem do erro
                return BadRequest($"Erro ao buscar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Método POST assíncrono para criar novos dados pessoais no banco
        /// Endpoint: POST /api/MeusDados
        /// Atributo HttpPost define que este método responde a requisições POST
        /// Recebe objeto MeusDados no corpo da requisição (deserializado automaticamente)
        /// </summary>
        /// <param name="novosDados">Objeto contendo dados pessoais a serem criados</param>
        /// <returns>ActionResult contendo dados criados ou erro de validação</returns>
        [HttpPost]
        public async Task<ActionResult<MeusDados>> CriarMeusDados(MeusDados novosDados)
        {
            try
            {
                // Valida se todos os campos obrigatórios estão preenchidos
                if (string.IsNullOrEmpty(novosDados.Nome) || 
                    string.IsNullOrEmpty(novosDados.RU) || 
                    string.IsNullOrEmpty(novosDados.Curso))
                {
                    // Retorna erro HTTP 400 se algum campo obrigatório estiver vazio
                    return BadRequest("Todos os campos são obrigatórios: Nome, RU e Curso.");
                }

                // Define a data de criação como a data/hora atual do sistema
                novosDados.DataCriacao = DateTime.Now;

                // Adiciona os novos dados ao contexto do Entity Framework
                _context.MeusDados.Add(novosDados);
                
                // Executa as mudanças no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();

                // Retorna status HTTP 201 Created com os dados criados e localização do recurso
                return CreatedAtAction(nameof(GetMeusDadosPorId), 
                    new { id = novosDados.Id }, novosDados);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status HTTP 400 Bad Request com mensagem do erro
                return BadRequest($"Erro ao criar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Método PUT assíncrono para atualizar dados pessoais existentes
        /// Endpoint: PUT /api/MeusDados/{id}
        /// Atributo HttpPut com template de rota que captura o ID da URL
        /// Recebe ID na URL e objeto MeusDados no corpo da requisição
        /// </summary>
        /// <param name="id">ID do registro a ser atualizado</param>
        /// <param name="dadosAtualizados">Objeto contendo dados atualizados</param>
        /// <returns>ActionResult contendo dados atualizados ou erro</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<MeusDados>> AtualizarMeusDados(int id, MeusDados dadosAtualizados)
        {
            try
            {
                // Verifica se o ID da URL corresponde ao ID do objeto recebido
                if (id != dadosAtualizados.Id)
                {
                    // Retorna erro HTTP 400 se os IDs não coincidirem
                    return BadRequest("ID da URL não corresponde ao ID dos dados.");
                }

                // Busca os dados existentes no banco de dados usando o ID
                var dadosExistentes = await _context.MeusDados.FindAsync(id);
                
                // Verifica se o registro foi encontrado
                if (dadosExistentes == null)
                    // Retorna status HTTP 404 Not Found se o registro não existir
                    return NotFound("Dados não encontrados.");

                // Atualiza os campos dos dados existentes com os novos valores
                dadosExistentes.Nome = dadosAtualizados.Nome;     // Atualiza nome
                dadosExistentes.RU = dadosAtualizados.RU;         // Atualiza RU
                dadosExistentes.Curso = dadosAtualizados.Curso;   // Atualiza curso

                // Salva as mudanças no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();

                // Retorna status HTTP 200 OK com os dados atualizados
                return Ok(dadosExistentes);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status HTTP 400 Bad Request com mensagem do erro
                return BadRequest($"Erro ao atualizar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Método DELETE assíncrono para remover dados pessoais do banco
        /// Endpoint: DELETE /api/MeusDados/{id}
        /// Atributo HttpDelete com template de rota que captura o ID da URL
        /// Remove permanentemente o registro do banco de dados
        /// </summary>
        /// <param name="id">ID do registro a ser removido</param>
        /// <returns>ActionResult com mensagem de sucesso ou erro</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarMeusDados(int id)
        {
            try
            {
                // Busca os dados a serem removidos usando o ID fornecido
                var dados = await _context.MeusDados.FindAsync(id);
                
                // Verifica se o registro foi encontrado no banco
                if (dados == null)
                    // Retorna status HTTP 404 Not Found se o registro não existir
                    return NotFound("Dados não encontrados.");

                // Remove os dados do contexto do Entity Framework
                _context.MeusDados.Remove(dados);
                
                // Executa a remoção no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();

                // Retorna status HTTP 200 OK com mensagem de confirmação
                return Ok("Dados removidos com sucesso.");
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status HTTP 400 Bad Request com mensagem do erro
                return BadRequest($"Erro ao deletar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Método POST especial para inicializar dados pessoais com informações pré-definidas
        /// Endpoint: POST /api/MeusDados/inicializar
        /// Atributo HttpPost com rota específica "inicializar"
        /// Cria automaticamente um registro com dados do estudante se não existir
        /// </summary>
        /// <returns>ActionResult contendo dados inicializados ou existentes</returns>
        [HttpPost("inicializar")]
        public async Task<ActionResult<MeusDados>> InicializarComMeusDados()
        {
            try
            {
                // Verifica se já existem dados pessoais no banco
                var dadosExistentes = await _context.MeusDados.FirstOrDefaultAsync();
                
                // Se já existem dados, retorna os dados existentes
                if (dadosExistentes != null)
                {
                    // Retorna status HTTP 200 OK com os dados já existentes
                    return Ok(dadosExistentes);
                }

                // Cria registro inicial com informações pré-definidas do estudante
                var meusDadosIniciais = new MeusDados
                {
                    Nome = "Lucas Martins Sorrentino",              // Nome completo pré-definido
                    RU = "4585828",                                 // RU pré-definido
                    Curso = "Análise e Desenvolvimento de Sistemas",  // Curso pré-definido
                    DataCriacao = DateTime.Now                      // Data atual de criação
                };

                // Adiciona os dados iniciais ao contexto
                _context.MeusDados.Add(meusDadosIniciais);
                // Salva no banco de dados
                await _context.SaveChangesAsync();

                // Retorna status HTTP 201 Created com os dados inicializados
                return CreatedAtAction(nameof(GetMeusDadosPorId), 
                    new { id = meusDadosIniciais.Id }, meusDadosIniciais);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status HTTP 400 Bad Request com mensagem do erro
                return BadRequest($"Erro ao inicializar dados: {ex.Message}");
            }
        }
    } // Fim da classe MeusDadosController
} // Fim do namespace LivrariaApi.Controllers
