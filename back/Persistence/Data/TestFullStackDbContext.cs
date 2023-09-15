using System.Reflection;
using Domine.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domine.Data;
public class TestFullStackDbContext : DbContext
{
    public TestFullStackDbContext(DbContextOptions<TestFullStackDbContext> options) : base(options)
    {
        // Constructor del contexto
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<UserRol> UsersRoles { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<HistorialRefreshToken> HistorialRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); //assembly está en código binario. contiene uno o más módulos o componentes de código compilado
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Habilitar el registro de datos sensibles
        optionsBuilder.EnableSensitiveDataLogging();
    }
}
