namespace CompanyEmployees.Utilities;

internal static class LoggingUtilities
{
    private static readonly string LogConfigFile;

    static LoggingUtilities() =>
        LogConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "/nlog.config");

    internal static void SetupLogging() =>
        LogManager.Setup().LoadConfigurationFromFile(LogConfigFile);
}