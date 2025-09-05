// Arquivo: Controllers/LivrosController.cs
// Descrição: Controller responsável por gerenciar operações CRUD de livros
// Simplificado para focar apenas na gestão de livros (autenticação removida)
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025 (atualizado)

// Importa funcionalidades do Entity Framework para acesso ao banco de dados
using LivrariaApi.Data;
// Importa classes base para controllers da Web API
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
    // Atributo que define a rota base do controller: "api/Livros"
    // [controller] será substituído pelo nome da classe sem "Controller"
    [Route("api/[controller]")]
    // Atributo que indica que esta classe é um controller de API
    // Habilita funcionalidades como validação automática de modelo e resposta de erro padronizada
    [ApiController]
    // Declaração da classe controller que herda de ControllerBase
    // ControllerBase fornece funcionalidades básicas para controllers de API
    public class LivrosController : ControllerBase
    {
        // Campo privado somente leitura que armazena a instância do contexto do banco de dados
        // O modificador 'readonly' garante que só pode ser atribuído no construtor
        private readonly DataContext _context;

        // Construtor da classe que recebe uma instância do DataContext
        // O ASP.NET Core injeta automaticamente a dependência através do sistema de DI
        public LivrosController(DataContext context)
        {
            // Atribui o contexto recebido ao campo privado da classe
            _context = context;
        }

        // --- ENDPOINTS PARA LIVROS ---
        // Atributo que indica que este método responde a requisições HTTP GET
        // Quando chamado, retorna todos os livros cadastrados no banco de dados
        [HttpGet]
        // Método assíncrono que retorna uma ActionResult contendo uma lista de livros
        // Task<T> indica operação assíncrona, ActionResult permite diferentes tipos de resposta HTTP
        public async Task<ActionResult<List<Livro>>> Get()
        {
            // Busca todos os registros da tabela Livros de forma assíncrona
            // ToListAsync() converte o resultado em uma lista e aguarda a operação do banco
            // Ok() retorna status HTTP 200 (sucesso) com os dados no corpo da resposta
            return Ok(await _context.Livros.ToListAsync());
        }

        // Atributo que indica que este método responde a requisições HTTP POST
        // Usado para criar/adicionar novos livros no banco de dados
        [HttpPost]
        // Método assíncrono que recebe um objeto Livro no corpo da requisição
        // O ASP.NET Core automaticamente deserializa o JSON em um objeto Livro
        public async Task<ActionResult<List<Livro>>> AddLivro(Livro livro)
        {
            // Adiciona o novo livro ao contexto do Entity Framework
            // Neste ponto, o livro ainda não foi salvo no banco, apenas marcado para inserção
            _context.Livros.Add(livro);

            // Executa todas as mudanças pendentes no banco de dados de forma assíncrona
            // É aqui que o livro é realmente inserido na tabela
            await _context.SaveChangesAsync();

            // Retorna status HTTP 200 com a lista atualizada de todos os livros
            // Isso permite que o cliente veja imediatamente o resultado da inserção
            return Ok(await _context.Livros.ToListAsync());
        }

        // Atributo que indica que este método responde a requisições HTTP PUT
        // Usado para atualizar/modificar livros existentes no banco de dados
        [HttpPut]
        // Método assíncrono que recebe um objeto Livro com os dados atualizados
        public async Task<ActionResult<List<Livro>>> UpdateLivro(Livro request)
        {
            // Busca no banco de dados um livro com o ID fornecido na requisição
            // FindAsync() é otimizado para busca por chave primária
            var dbLivro = await _context.Livros.FindAsync(request.Id);

            // Verifica se o livro foi encontrado no banco de dados
            if (dbLivro == null)
                // Retorna erro HTTP 400 (Bad Request) se o livro não existir
                return BadRequest("Livro não encontrado.");

            // Atualiza as propriedades do livro encontrado com os novos valores
            // Modifica o título do livro com o valor recebido na requisição
            dbLivro.Titulo = request.Titulo;
            // Modifica o autor do livro com o valor recebido na requisição
            dbLivro.Autor = request.Autor;
            // Modifica o ano do livro com o valor recebido na requisição
            dbLivro.Ano = request.Ano;
            // Modifica a editora do livro com o valor recebido na requisição
            dbLivro.Editora = request.Editora;
            // Modifica a cidade de publicação do livro com o valor recebido na requisição
            dbLivro.Cidade = request.Cidade;

            // Salva todas as alterações no banco de dados de forma assíncrona
            // O Entity Framework detecta automaticamente as propriedades modificadas
            await _context.SaveChangesAsync();

            // Retorna status HTTP 200 com a lista atualizada de todos os livros
            return Ok(await _context.Livros.ToListAsync());
        }

        // Atributo que indica que este método responde a requisições HTTP DELETE
        // {id} na rota indica que o ID será passado como parâmetro na URL
        // Exemplo: DELETE /api/Livros/5 (onde 5 é o ID do livro a ser excluído)
        [HttpDelete("{id}")]
        // Método assíncrono que recebe o ID do livro a ser excluído como parâmetro
        public async Task<ActionResult<List<Livro>>> Delete(int id)
        {
            // Busca no banco de dados o livro com o ID fornecido
            // FindAsync() localiza o registro pela chave primária de forma assíncrona
            var dbLivro = await _context.Livros.FindAsync(id);

            // Verifica se o livro foi encontrado no banco de dados
            if (dbLivro == null)
                // Retorna erro HTTP 400 (Bad Request) se o livro não existir
                return BadRequest("Livro não encontrado.");

            // Marca o livro para remoção no contexto do Entity Framework
            // O registro ainda não foi deletado do banco, apenas marcado para exclusão
            _context.Livros.Remove(dbLivro);

            // Executa a remoção no banco de dados de forma assíncrona
            // É aqui que o registro é efetivamente deletado da tabela
            await _context.SaveChangesAsync();

            // Retorna status HTTP 200 com a lista atualizada de livros (sem o livro excluído)
            // Permite que o cliente veja imediatamente o resultado da exclusão
            return Ok(await _context.Livros.ToListAsync());
        } // Fim do método Delete
    } // Fim da classe LivrosController
} // Fim do namespace LivrariaApi.Controllers

