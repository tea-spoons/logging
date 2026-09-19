
namespace TeaSpoons.Logging
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using static TeaSpoons.Logging.Logger;

    /// <summary>
    /// A category for logging.
    /// Each game should define its own semantic categories to sort logs into.
    /// </summary>
    public class LogCategory
    {
        private static readonly Dictionary<string, LogCategory> _allCategories = new Dictionary<string, LogCategory>();
        internal static IEnumerable<LogCategory> allCategories => _allCategories.Values;

        internal static readonly LogLevel defaultMaxLogLevel = LogLevel.Info;
        /// <summary>
        /// The maximum log level that ANY category forwards logs for.
        /// Everything at a higher level will be ignored.
        /// </summary>
        internal static LogLevel globalMaxLogLevel = LogLevel.Highest;
        
        private static readonly StringBuilder toStringBuilder = new();

        internal static void UpdateAllLoggers(LogLevel level)
        {
            foreach (var category in allCategories)
            {
                category.UpdateLogger(level);
            }
        }

        internal static void UpdateAllLoggers()
        {
            foreach (var category in allCategories)
            {
                category.UpdateLoggers();
            }
        }

        /// <summary>
        /// The error level for when something went wrong.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no logging registered to this level in this category.
        /// Always null check before calling <c>Log</c>.
        /// </remarks>
        public Logger Error => GetLoggerIfBelowMaxLogLevel(LogLevel.Error);
        /// <summary>
        /// The warning level for potentially suspiscious occurrences.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no logging registered to this level in this category.
        /// Always null check before calling <c>Log</c>.
        /// </remarks>
        public Logger Warning => GetLoggerIfBelowMaxLogLevel(LogLevel.Warning);
        /// <summary>
        /// The info level for useful information.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no logging registered to this level in this category.
        /// Always null check before calling <c>Log</c>.
        /// </remarks>
        public Logger Info => GetLoggerIfBelowMaxLogLevel(LogLevel.Info);
        /// <summary>
        /// The debug level for additional information that might be useful during an investigation.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no logging registered to this level in this category.
        /// Always null check before calling <c>Log</c>.
        /// </remarks>
        public Logger Debug => GetLoggerIfBelowMaxLogLevel(LogLevel.Debug);
        /// <summary>
        /// The trace level for information that can be used during tracing.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no logging registered to this level in this category.
        /// Always null check before calling <c>Log</c>.
        /// </remarks>
        public Logger Trace => GetLoggerIfBelowMaxLogLevel(LogLevel.Trace);

        public readonly string Name;
        /// <summary>
        /// The name of this category, formatted as a prefix for logging.
        /// </summary>
        /// <example>
        /// Name = "Combat"
        /// LogPrefix = "[Combat] "
        /// </example>
        public readonly string LogPrefix;
        private readonly Logger[] loggers = new Logger[LogLevel.AvailableCount];

        /// <summary>
        /// The maximum log level this category forwards logs for.
        /// Everything at a higher level will be ignored.
        /// </summary>
        internal LogLevel customMaxLogLevel = defaultMaxLogLevel;
        /// <summary>
        /// The effective max log level, defined as the lowest applicable level.
        /// </summary>
        internal LogLevel effectiveMaxLogLevel
        {
            get
            {
#if UNITY_EDITOR
                if (EditorMaxLogLevelOverrides.TryGet(Name, out var editorOverrideMaxLogLevel))
                {
                    return editorOverrideMaxLogLevel;
                }
#endif
                return LogLevel.GetLower(globalMaxLogLevel, customMaxLogLevel);
            }
        }


        public LogCategory(string name)
        {
            Name = name;
            LogPrefix = GenerateLogPrefix();

            _allCategories[name] = this;

            UpdateLoggers();
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is LogCategory other)
            {
                return other.Name == Name;
            }
            return false;
        }

        internal void AddLogAction(LogLevel level, LogAction logAction)
        {
            GetOrCreateLogger(level).customLogActions.Add(logAction);
        }

        internal void RemoveLogAction(LogLevel level, LogAction logAction)
        {
            var logger = GetLogger(level);

            if (logger != null)
            {
                logger.customLogActions.Remove(logAction);

                if (!logger.hasLogAction && !level.hasAnyLogAction)
                {
                    loggers[level] = null;
                }
            }
        }

        private Logger GetLogger(LogLevel level)
        {
            if (level == null) throw new System.ArgumentNullException(nameof(level));

            return loggers[level];
        }

        private Logger GetOrCreateLogger(LogLevel level)
        {
            if (level == null) throw new System.ArgumentNullException(nameof(level));

            var logger = loggers[level];

            if (logger == null)
            {
                logger = new Logger(this, level);
                loggers[level] = logger;
            }

            return logger;
        }

        private Logger GetLoggerIfBelowMaxLogLevel([NotNull] LogLevel level)
        {
            if (level > effectiveMaxLogLevel) return null;

            return loggers[level];
        }

        private void UpdateLoggers()
        {
            UpdateLogger(LogLevel.Error);
            UpdateLogger(LogLevel.Warning);
            UpdateLogger(LogLevel.Info);
            UpdateLogger(LogLevel.Debug);
            UpdateLogger(LogLevel.Trace);
        }

        private void UpdateLogger(LogLevel level)
        {
            var logger = loggers[level];

            if (logger != null)
            {
                if (!logger.hasLogAction && !level.hasAnyLogAction)
                {
                    loggers[level] = null;
                }
            }
            else
            {
                if (level.hasAnyLogAction)
                {
                    logger = new Logger(this, level);
                    loggers[level] = logger;
                }
            }
        }

        private string GenerateLogPrefix()
        {
            toStringBuilder.Clear();
#if UNITY_EDITOR
            toStringBuilder.Append("<color=#");
            toStringBuilder.Append(CategoryColor.GetHex(this));
            toStringBuilder.Append("><b>");
#endif
            toStringBuilder.Append("[");
            toStringBuilder.Append(Name);
#if UNITY_EDITOR
            toStringBuilder.Append("]</b></color> ");
#else
            toStringBuilder.Append("] ");
#endif
            return toStringBuilder.ToString();
        }
    }
}
