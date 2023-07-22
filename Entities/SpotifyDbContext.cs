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
            modelBuilder.Entity<User>()
            .Property(entity => entity.Offers)
            .HasConversion<int>();

            modelBuilder.Entity<User>()
            .Property(entity => entity.ShareInformation)
            .HasConversion<int>();

            modelBuilder.Entity<User>()
            .Property(entity => entity.Terms)
            .HasConversion<int>();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Nickname)
            .IsUnique();
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

        public async Task<bool> UserExists(string email, string nickname)
        {
            return await Users.AnyAsync(x => x.Email == email || x.Nickname == nickname);
        }
    }
}