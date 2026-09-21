using Microsoft.EntityFrameworkCore;
using {{ProjectNamespace}}.Application.Status;

namespace {{ProjectNamespace}}.Infrastructure.Persistence;

/// <summary>EF Core implementation of <see cref="IDatabaseProbe"/>.</summary>
internal sealed class DatabaseProbe : IDatabaseProbe
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseProbe(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> CanConnectAsync(CancellationToken cancellationToken) =>
        _dbContext.Database.CanConnectAsync(cancellationToken);
}
