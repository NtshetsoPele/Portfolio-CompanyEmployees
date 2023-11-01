namespace CompanyEmployees.ContextFactory;

// Creates a derived DbContext instance at design time, helping with migrations.
public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] _)
    {
        var config = GetConfiguration();

        var builder = GetDbContextOptionsBuilder(config);

        return new RepositoryContext(builder.Options);
    }

    private static IConfigurationRoot GetConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

    private static DbContextOptionsBuilder<RepositoryContext> GetDbContextOptionsBuilder(IConfiguration config) =>
        new DbContextOptionsBuilder<RepositoryContext>()
            .UseSqlServer(connectionString: config.GetConnectionString(name: "sqlConnection"),
                sqlServerOptionsAction: (SqlServerDbContextOptionsBuilder b) => b.MigrationsAssembly("CompanyEmployees"));
}