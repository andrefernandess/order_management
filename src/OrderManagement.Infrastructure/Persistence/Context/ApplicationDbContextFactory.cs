using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderManagement.Infrastructure.Persistence.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Connection string usada apenas para criar migrations
        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=OrderManagement;User Id=sa;Password=SuaSenha@123;TrustServerCertificate=True");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
