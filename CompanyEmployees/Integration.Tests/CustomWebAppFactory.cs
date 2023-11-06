using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
namespace Integration.Tests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((IServiceCollection services) =>
        {
            ServiceDescriptor? dbContextOptionsDescriptor =
                services.SingleOrDefault((ServiceDescriptor serviceDescriptor) =>
                    serviceDescriptor.ServiceType == typeof(DbContextOptions<RepositoryContext>));

            if (dbContextOptionsDescriptor != null)
            {
                services.Remove(dbContextOptionsDescriptor);
            }

            services.AddDbContext<RepositoryContext>(optionsAction: (DbContextOptionsBuilder options) =>
            {
                options.UseInMemoryDatabase(databaseName: "InMemoryDb");
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            using RepositoryContext context = serviceScope.ServiceProvider.GetRequiredService<RepositoryContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated(); // Includes seeding data
        });
    }
}