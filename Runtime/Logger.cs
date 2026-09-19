#if UNITY_2022_2_OR_NEWER || (UNITY_2021_3_OR_NEWER && !UNITY_2022_1_OR_NEWER)
#define HIDE_IN_CALLSTACK
#endif

namespace TeaSpoons.Logging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A logger representing a combination of <see cref="LogCategory"/> and <see cref="LogLevel"/>.
    /// </summary>
    public class Logger
    {
        public delegate void LogAction(in LogEntry entry);

        internal readonly List<LogAction> customLogActions = new List<LogAction>();
        internal bool hasLogAction => customLogActions.Count > 0 || level.hasAnyLogAction;
        private readonly LogCategory category;
        private readonly LogLevel level;

        internal Logger(LogCategory category, LogLevel level)
        {
            this.category = category;
            this.level = level;
        }

#line hidden
        /// <summary>
        /// Logs the given <param <paramref name="message"/>.
        /// </summary>
        /// <param name="additionalData">Optional additional data that your custom log actions can freely process.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(string message, in ILogData additionalData = null)
        {
            Log(message, null, additionalData, 0);
        }

        /// <summary>
        /// Logs the given <param <paramref name="exception"/>.
        /// </summary>
        /// <param name="additionalData">Optional additional data that your custom log actions can freely process.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(Exception exception, in ILogData additionalData = null)
        {
            Log(null, exception, additionalData, 0);
        }

        /// <summary>
        /// Logs the given <paramref name="message"/>.
        /// </summary>
        /// <param name="flags">Optional flags that your custom log actions can freely interpret.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(string message, int flags)
        {
            Log(message, null, null, flags);
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="flags">Optional flags that your custom log actions can freely interpret.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(Exception exception, int flags)
        {
            Log(null, exception, null, flags);
        }

        /// <summary>
        /// Logs the given <paramref name="message"/>.
        /// </summary>
        /// <param name="additionalData">Optional additional data that your custom log actions can freely process.</param>
        /// <param name="flags">Optional flags that your custom log actions can freely interpret.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(string message, in ILogData additionalData, int flags)
        {
            Log(message, null, null, flags);
        }

        /// <summary>
        /// Logs the given <paramref name="exception"/>.
        /// </summary>
        /// <param name="additionalData">Optional additional data that your custom log actions can freely process.</param>
        /// <param name="flags">Optional flags that your custom log actions can freely interpret.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(Exception exception, in ILogData additionalData, int flags)
        {
            Log(null, exception, null, flags);
        }

        /// <summary>
        /// Logs the given <paramref name="message"/> and <paramref name="exception"/>.
        /// </summary>
        /// <param name="additionalData">Optional additional data that your custom log actions can freely process.</param>
        /// <param name="flags">Optional flags that your custom log actions can freely interpret.</param>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        public void Log(string message, Exception exception, in ILogData additionalData = null, int flags = 0)
        {
            var entry = new LogEntry(message, exception, additionalData, flags, category, level);

            foreach (var logAction in customLogActions)
            {
                logAction(entry);
            }

            level.InvokeDefaultLogActions(entry);
        }
#line default
    }
}
