// Data/DataContext.cs
using Microsoft.EntityFrameworkCore;

namespace SuperHeroApi.Data // Este é o namespace que estava faltando
{
    public class DataContext : DbContext // Herda da classe DbContext do Entity Framework
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        // Mapeia a classe SuperHero para uma tabela "SuperHeroes" no banco de dados
        public DbSet<SuperHero> SuperHeroes { get; set; } = null!;

    }
}