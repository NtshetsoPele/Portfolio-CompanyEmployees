namespace Repository;

// Mechanism to seed data and retry 6 times if the SQL server is not available
public static class MigrationsManager
{
    private static int _numberOfRetries;

    public static IHost MigrateDatabase(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
        try
        {
            dbContext.Database.Migrate();
        }
        catch (SqlException _)
        {
            if (_numberOfRetries >= 6)
            {
                throw;
            }

            Thread.Sleep(millisecondsTimeout: 10_000);
            _numberOfRetries++;
            Console.WriteLine("The server was not found or was not accessible. " +
                              $"Retrying... #{_numberOfRetries}.");
            MigrateDatabase(host);

            throw;
        }

        return host;
    }
}