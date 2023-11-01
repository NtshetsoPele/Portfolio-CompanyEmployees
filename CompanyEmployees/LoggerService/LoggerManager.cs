using NLog;
using Contracts.Logging;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public void LogDebug(string message) => Logger.Debug(message: "\n" + message);
    public void LogError(string message) => Logger.Error(message: "\n" + message);
    public void LogInfo(string message) => Logger.Info(message: "\n" + message);
    public void LogWarn(string message) => Logger.Warn(message: "\n" + message);
}