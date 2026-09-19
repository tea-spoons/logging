#if UNITY_2022_2_OR_NEWER || (UNITY_2021_3_OR_NEWER && !UNITY_2022_1_OR_NEWER)
#define HIDE_IN_CALLSTACK
#endif

namespace TeaSpoons.Logging
{
    using System;
    using static TeaSpoons.Logging.Logger;

    /// <summary>
    /// A logging level to categorize any given log with.
    /// Each instance is for one specific nature of events.
    /// </summary>
    public class LogLevel
    {
        internal const int AvailableCount = 5;

        /// <summary>
        /// A log level that is used to set a max log level for a <see cref="LogCategory"/> that is completely muted.
        /// </summary>
        internal static LogLevel None { get; private set; } = new LogLevel(-1);

        /// <summary>
        /// The error level for when something went wrong.
        /// </summary>
        public static LogLevel Error { get; private set; } = new LogLevel(0, LogUnityError);
        /// <summary>
        /// The warning level for potentially suspiscious occurrences.
        /// </summary>
        public static LogLevel Warning { get; private set; } = new LogLevel(1, LogUnityWarning);
        /// <summary>
        /// The info level for useful information.
        /// </summary>
        public static LogLevel Info { get; private set; } = new LogLevel(2, LogUnity);
        /// <summary>
        /// The debug level for additional information that might be useful during an investigation.
        /// </summary>
#if UNITY_EDITOR
        public static LogLevel Debug { get; private set; } = new LogLevel(3, LogUnityDebug);
#else
        public static LogLevel Debug { get; private set; } = new LogLevel(3);
#endif
        /// <summary>
        /// The trace level for information that can be used during tracing.
        /// </summary>
#if UNITY_EDITOR
        public static LogLevel Trace { get; private set; } = new LogLevel(4, LogUnityTrace);
#else
        public static LogLevel Trace { get; private set; } = new LogLevel(4);
#endif

        /// <summary>
        /// The highest available log level (<see cref="Trace"/>).
        /// </summary>
        public static LogLevel Highest => Trace;


        private readonly short index;

        private readonly LogAction unityLogAction;
        private bool canUseUnityLogAction => !Setup.UnityLogHandler.IsUsed && unityLogAction != null;

        internal LogAction defaultCustomLogAction;
        /// <summary>
        /// <c>true</c> if there is any kind of valid <see cref="LogAction"/> defined for the entire level.
        /// </summary>
        internal bool hasAnyLogAction => defaultCustomLogAction != null || canUseUnityLogAction;

#line hidden
        private LogLevel(short index, LogAction unityLogAction = null)
        {
            this.index = index;

            this.unityLogAction = unityLogAction;
        }

        /// <summary>
        /// Invokes the assigned <see cref="defaultCustomLogAction"/> if available.
        /// If it's not, invokes the default Unity log action.
        /// </summary>
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        internal void InvokeDefaultLogActions(in LogEntry entry)
        {
            if (defaultCustomLogAction != null)
            {
                defaultCustomLogAction.Invoke(entry);
            }
            else if (canUseUnityLogAction)
            {
                unityLogAction(entry);
            }
        }

        #region Default Unity LogActions
#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        private static void LogUnityError(in LogEntry logEntry)
        {
            UnityEngine.Debug.LogError(logEntry.ToString());
        }

#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        private static void LogUnityWarning(in LogEntry logEntry)
        {
            UnityEngine.Debug.LogWarning(logEntry.ToString());
        }

#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        private static void LogUnity(in LogEntry logEntry)
        {
            UnityEngine.Debug.Log(logEntry.ToString());
        }

#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        private static void LogUnityDebug(in LogEntry logEntry)
        {
            UnityEngine.Debug.Log(logEntry.ToString("[D]"));
        }

#if HIDE_IN_CALLSTACK
        [UnityEngine.HideInCallstack]
#endif
        private static void LogUnityTrace(in LogEntry logEntry)
        {
            UnityEngine.Debug.Log(logEntry.ToString("[T]"));
        }
        #endregion
#line default

        #region Operators Overloads and Operations
        public static bool operator >=(LogLevel lhs, LogLevel rhs)
        {
            return lhs.index >= rhs.index;
        }

        public static bool operator <=(LogLevel lhs, LogLevel rhs)
        {
            return lhs.index <= rhs.index;
        }

        public static bool operator >(LogLevel lhs, LogLevel rhs)
        {
            return lhs.index > rhs.index;
        }

        public static bool operator <(LogLevel lhs, LogLevel rhs)
        {
            return lhs.index < rhs.index;
        }

        public static implicit operator short(LogLevel level)
        {
            return level.index;
        }

        internal static LogLevel GetLower(LogLevel a, LogLevel b)
        {
            return a <= b ? a : b;
        }
        #endregion
    }
}
