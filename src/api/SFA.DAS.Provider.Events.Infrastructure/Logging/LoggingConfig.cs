using System.Configuration;
using System.Linq;
using Microsoft.Azure;
using NLog;
using NLog.Targets;

namespace SFA.DAS.Provider.Events.Infrastructure.Logging
{
    public static class LoggingConfig
    {
        public static void ConfigureLogging()
        {
            ConfigureLoggingLevel();
            ConfigureConnectionString();

            LogManager.ReconfigExistingLoggers();
        }

        private static void ConfigureConnectionString()
        {
            var targets = LogManager.Configuration.AllTargets.Where(t => t is DatabaseTarget).Cast< DatabaseTarget>().ToArray();
            foreach (var target in targets)
            {
                target.ConnectionString = ConfigurationManager.AppSettings["LoggingConnectionString"];
            }
        }

        private static void ConfigureLoggingLevel()
        {
            var loggingLevels = LogLevel.AllLoggingLevels.ToList();
            var minLevel = GetLogLevelFromConfigurationManager();
            var levelIndex = loggingLevels.IndexOf(minLevel);

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                for (var i = 0; i < loggingLevels.Count; i++)
                {
                    var level = loggingLevels[i];
                    var hasLevel = rule.IsLoggingEnabledForLevel(level);
                    if (i < levelIndex && hasLevel)
                    {
                        rule.DisableLoggingForLevel(level);
                    }
                    else if (i >= levelIndex && !hasLevel)
                    {
                        rule.EnableLoggingForLevel(level);
                    }
                }
            }
        }
        private static LogLevel GetLogLevelFromConfigurationManager()
        {
            var settingValue = ConfigurationManager.AppSettings["LogLevel"];
            return LogLevel.FromString(settingValue);
        }
    }
}
