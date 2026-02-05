using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Infrastructure.Persistence.Context;

namespace OrderManagement.Infrastructure.Persistence.Repositories;

public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email.Value == email.ToLowerInvariant(), ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .AnyAsync(c => c.Email.Value == email.ToLowerInvariant(), ct);
    }

    public async Task<IReadOnlyList<Customer>> GetActiveCustomersAsync(CancellationToken ct = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }
}
