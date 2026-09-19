
namespace TeaSpoons.Logging.Setup
{
    using System;
    using static TeaSpoons.Logging.Logger;

    /// <summary>
    /// Static methods for the initial setup or runtime updating of <see cref="LogLevel"/>s and <see cref="LogCategory"/>s.
    /// </summary>
    public static class LoggingSetup
    {
        private static LogCategory unhandledExceptionHandler;

        /// <summary>
        /// Sets the default <see cref="LogAction"/> for the given <paramref name="level"/>.
        /// This action will be invoked whenever something is logged to that <see cref="LogLevel"/>, regardless of the <see cref="LogCategory"/>.
        /// </summary>
        /// <remarks>
        /// Setting <paramref name="logAction"/> to a non-null value will disable default Unity console logging behavior.
        /// </remarks>
        public static void SetDefaultLogAction(LogLevel level, LogAction logAction)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));

            var hadAnyAction = level.hasAnyLogAction;
            
            level.defaultCustomLogAction = logAction;

            if (level.hasAnyLogAction != hadAnyAction)
            {
                LogCategory.UpdateAllLoggers(level);
            }
        }

        /// <summary>
        /// Adds a custom <see cref="LogAction"/> to the given <paramref name="category"/>/<paramref name="level"/> combination.
        /// </summary>
        public static void AddCustomLogAction(LogCategory category, LogLevel level, LogAction logAction)
        {
            if (category == null || level == null) throw new ArgumentNullException(category == null ? nameof(category) : nameof(level));

            category.AddLogAction(level, logAction);
        }

        /// <summary>
        /// Removes a custom <see cref="LogAction"/> from the given <paramref name="category"/>/<paramref name="level"/> combination.
        /// </summary>
        public static void RemoveCustomLogAction(LogCategory category, LogLevel level, LogAction logAction)
        {
            if (category == null || level == null) throw new ArgumentNullException(category == null ? nameof(category) : nameof(level));

            category.RemoveLogAction(level, logAction);
        }

        /// <summary>
        /// Sets the maximum (inclusive) log <paramref name="level"/> for the given <paramref name="category"/>.
        /// All logs to a higher <see cref="LogLevel"/> will be ignored.
        /// </summary>
        /// <remarks>
        /// This does NOT overrule the global max log level.
        /// </remarks>
        public static void SetMaxLogLevel(LogCategory category, LogLevel level)
        {
            if (category == null || level == null) throw new ArgumentNullException(category == null ? nameof(category) : nameof(level));

            category.customMaxLogLevel = level;
        }

        /// <summary>
        /// Disables logging for the given <paramref name="category"/>.
        /// </summary>
        /// <remarks>
        /// The category can be re-enabled by using <see cref="SetMaxLogLevel(LogCategory, LogLevel)"/>.
        /// </remarks>
        public static void MuteLogCategory(LogCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            category.customMaxLogLevel = LogLevel.None;
        }

        /// <summary>
        /// Sets the global maximum (inclusive) log <paramref name="level"/>.
        /// All logs to a higher <see cref="LogLevel"/> will be ignored.
        /// </summary>
        public static void SetGlobalMaxLogLevel(LogLevel level)
        {
            LogCategory.globalMaxLogLevel = level;
        }

        /// <summary>
        /// Completely disables all logging.
        /// </summary>
        /// <remarks>
        /// Logging can be re-enabled by using <see cref="SetGlobalMaxLogLevel(LogLevel)"/>.
        /// </remarks>
        public static void MuteLogging()
        {
            LogCategory.globalMaxLogLevel = LogLevel.None;
        }

        /// <summary>
        /// Routes otherwise unhandled exceptions to <paramref name="category"/>.
        /// </summary>
        /// <param name="category">
        /// The <see cref="LogCategory"/> to send the exceptions to.
        /// Pass <c>null</c> to stop handling unhandled exceptions.
        /// </param>
        public static void RouteUnhandledExceptionsTo(LogCategory category)
        {
            var hadHandler = unhandledExceptionHandler != null;
            var hasHandler = category != null;

            unhandledExceptionHandler = category;

            if (hadHandler != hasHandler)
            {
                if (hasHandler)
                {
                    AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
                }
                else
                {
                    AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
                }
            }
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.ExceptionObject is Exception exception)
            {
                unhandledExceptionHandler.Error?.Log(exception);
            }
        }
    }
}
