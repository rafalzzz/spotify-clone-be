using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using SpotifyAPI.Variables;

namespace SpotifyAPI.Entities
{
    public class SpotifyDbContext : DbContext
    {
        public SpotifyDbContext(DbContextOptions<SpotifyDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            Env.Load();

            var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariables.ConnectionString);
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}