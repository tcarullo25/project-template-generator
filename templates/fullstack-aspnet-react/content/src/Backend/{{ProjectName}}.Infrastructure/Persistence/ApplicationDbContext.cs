using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace {{ProjectNamespace}}.Infrastructure.Persistence;

/// <summary>
/// The application's single EF Core context.
/// <para>
/// It has no entities yet. Add <c>DbSet&lt;T&gt;</c> properties for your domain
/// types and put their mapping in <c>IEntityTypeConfiguration&lt;T&gt;</c>
/// classes in this assembly — <see cref="OnModelCreating"/> picks them up
/// automatically, so this file does not grow as the model does.
/// </para>
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
