global using SuperHeroApi.Data;
global using Microsoft.EntityFrameworkCore;
global using SuperHeroApi;   // chamar a pasta que contem a dependencia de objetos


var MyCorsConfig = "_myCorsConfig"; // nome da pol�tica
var builder = WebApplication.CreateBuilder(args);

// Add services to the container: chamada do m�todo servi�os CORS ao container de servi�o da API

/*builder.Services.AddCors(options =>
{
    // politica CORS padr�o
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://example.com",
                               "http://www.contoso.com");
        });
});*/

// politica CORS com m�todo de extens�o - RequireCors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsConfig,
        policy =>
        {
            policy.WithOrigins("http://example.com",
                                "http://contoso.com");
        });
});

builder.Services.AddControllers();
builder.Services.AddScoped<IDependencia_obj, Dependencia_obj>();// para localizar no servidor o objetos de dependencias. 
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  //  chamado do swagger no servidor.

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();



// Configure the HTTP request pipeline.

app.UseSwagger();   // utilizando swagger - microservi�os
app.UseSwaggerUI(); // especifica��o de linguagem para se descrever APIS usandp REST.}// refeenciada tamb�m como OPENAPI

//app.UseCors(MyCorsConfig); // m�todo de extens�o da pol�tica CORS e add o middleware
app.UseCors();  // aplica a politica CORS padr�o a todos os pontos de extremidade do controlador.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/echo",
        context => context.Response.WriteAsync("echo"))
             .RequireCors(MyCorsConfig); // pontos de extremidade e de controlador permitem solicita��es de origem cruzada utiizando a pol�tica especificada;

    endpoints.MapControllers()
              .RequireCors(MyCorsConfig);
   // endpoints.MapGet("/echo2",
     //   context => context.Response.WriteAsync("echo2")); // pontos de extremidade e Razor p�ginas n�o permitem solicita��es de origem cruzada;
});

app.MapControllers();

app.Run();
