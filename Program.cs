// Arquivo: Program.cs
// Descrição: Ponto de entrada da aplicação ASP.NET Core - configuração de serviços e middleware
// Inclui configuração JWT, CORS, Entity Framework e injeção de dependências
// Autor: Lucas Martins Sorrentino - RU: 4585828
// Data: 04/09/2025 (atualizado com autenticação JWT)

// Importações globais para uso em todo o projeto
global using LivrariaApi.Data;              // Contexto de dados global
global using Microsoft.EntityFrameworkCore;  // Entity Framework global
global using LivrariaApi;                    // Dependências principais global

// Importações específicas para autenticação JWT
using LivrariaApi.Services;                  // Serviços da aplicação
using Microsoft.AspNetCore.Authentication.JwtBearer;  // Autenticação JWT
using Microsoft.IdentityModel.Tokens;       // Tokens de segurança
using Microsoft.OpenApi.Models;             // Modelos OpenAPI para Swagger
using System.Text;                           // Codificação de texto

// Declaração de uma constante string que define o nome da política CORS customizada
var MyCorsConfig = "_myCorsConfig"; // nome da política
// Criação do builder para configurar e construir a aplicação web ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// Add services to the container: chamada do método serviços CORS ao container de serviço da API

/*builder.Services.AddCors(options =>
{
    // politica CORS padrão
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://example.com",
                               "http://www.contoso.com");
        });
});*/

// Configuração da política CORS personalizada usando método de extensão - RequireCors
// Esta configuração permite requisições de origem cruzada de domínios específicos
builder.Services.AddCors(options =>
{
    // Adiciona uma política CORS nomeada com configurações específicas
    options.AddPolicy(name: MyCorsConfig,
        policy =>
        {
            // Define quais origens são permitidas para fazer requisições à API
            // Especifica os domínios que podem acessar a API através de CORS
            policy.WithOrigins("http://example.com",
                                "http://contoso.com");
        });
});

// Adiciona o serviço de controllers ao container de dependências da aplicação
// Permite que a aplicação reconheça e utilize controllers para roteamento
builder.Services.AddControllers();

// Registra o serviço de autenticação no container de DI
builder.Services.AddScoped<IAuthService, AuthService>();

// Registra a interface IDependencia_obj e sua implementação no container de DI
// AddScoped cria uma instância por requisição HTTP, garantindo isolamento de dados
builder.Services.AddScoped<IDependencia_obj, Dependencia_obj>();// para localizar no servidor o objetos de dependencias. 
// Configura o contexto do Entity Framework para usar SQLite como banco de dados
// Obtém a string de conexão do arquivo de configuração appsettings.json
builder.Services.AddDbContext<DataContext>(options =>
{
    // Define SQLite como provedor de banco de dados usando a connection string "DefaultConnection"
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Adiciona o serviço para exploração de endpoints da API (necessário para Swagger)
builder.Services.AddEndpointsApiExplorer();
// Adiciona o gerador Swagger para documentação automática da API com suporte JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Livraria API", 
        Version = "v1",
        Description = "API para gerenciamento de livraria com autenticação JWT"
    });
    
    // Configuração do esquema de segurança JWT para Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Aplica o esquema de segurança a todos os endpoints
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Configuração da autenticação JWT
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"] ?? "MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%";
var key = Encoding.ASCII.GetBytes(jwtSecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "LivrariaApi",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "LivrariaApi",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Constrói a aplicação web com todas as configurações definidas anteriormente
var app = builder.Build();
// Força o redirecionamento de HTTP para HTTPS para maior segurança
app.UseHttpsRedirection();
// Habilita o servimento de arquivos estáticos (CSS, JS, imagens, etc.)
app.UseStaticFiles();
// Configura o middleware de roteamento para processar requisições HTTP
app.UseRouting();

// Configure the HTTP request pipeline.

// Habilita o middleware Swagger para gerar documentação da API em JSON
app.UseSwagger();   // utilizando swagger - microserviços
// Habilita a interface web do Swagger UI para visualizar e testar a API
app.UseSwaggerUI(); // especificação de linguagem para se descrever APIS usando REST.}// referenciada também como OPENAPI

//app.UseCors(MyCorsConfig); // método de extensão da política CORS e add o middleware
// Aplica a política CORS padrão a todos os endpoints do controlador
app.UseCors();  // aplica a politica CORS padrão a todos os pontos de extremidade do controlador.

// Força novamente o redirecionamento HTTPS (duplicado - pode ser removido)
app.UseHttpsRedirection();

// Habilita o middleware de autenticação JWT
app.UseAuthentication();

// Habilita o middleware de autorização para controle de acesso
app.UseAuthorization();

// Configura endpoints personalizados com requisitos CORS específicos
app.UseEndpoints(endpoints =>
{
    // Mapeia um endpoint GET simples que retorna "echo"
    // Aplica a política CORS personalizada especificamente a este endpoint
    endpoints.MapGet("/echo",
        context => context.Response.WriteAsync("echo"))
             .RequireCors(MyCorsConfig); // pontos de extremidade e de controlador permitem solicitações de origem cruzada utilizando a política especificada;

    // Mapeia todos os controllers da aplicação e aplica a política CORS personalizada
    endpoints.MapControllers()
              .RequireCors(MyCorsConfig);
   // endpoints.MapGet("/echo2",
     //   context => context.Response.WriteAsync("echo2")); // pontos de extremidade e Razor páginas não permitem solicitações de origem cruzada;
});

// Mapeia os controllers para roteamento automático (duplicado - pode ser removido)
app.MapControllers();

// Inicia a aplicação e começa a escutar por requisições HTTP
app.Run();
