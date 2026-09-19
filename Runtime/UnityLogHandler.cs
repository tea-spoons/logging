
namespace TeaSpoons.Logging.Setup
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This <see cref="ILogHandler"/> forwards any incoming Unity logs to its assigned <see cref="LogCategory"/>.
    /// </summary>
    public class UnityLogHandler : ILogHandler
    {
        /// <summary>
        /// An instance of this class is used instead of an actual <see cref="UnityLogHandler"/> if <see cref="CreateAndAssign(LogCategory)"/> is called with <c>null</c>.
        /// </summary>
        private class EmptyHandler : ILogHandler
        {
            void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
            {
            }

            void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
            }
        }

        private static ILogHandler defaultUnityLogHandler;
        private static ILogHandler instance;

        /// <summary>
        /// <c>True</c> when a <see cref="UnityLogHandler"/> is in use.
        /// <c>False</c> when Unity logs are handled by Unity in the default way.
        /// </summary>
        internal static bool IsUsed => instance != null;

        private LogCategory target;

        /// <summary>
        /// Routes <see cref="Debug"/> log calls to <paramref name="category"/>.
        /// </summary>
        /// <param name="target">The <see cref="LogCategory"/> to reroute Unity logs to. You can pass <c>null</c> to disable Unity logs without rerouting them anywhere.</param>
        /// <remarks>
        /// To avoid endless recursion, make sure <paramref name="category"/> does not have a <see cref="LogAction"/> that calls any <see cref="Debug"/> log method.
        /// </remarks>
        public static void CreateAndAssign(LogCategory target)
        {
            if (defaultUnityLogHandler == null)
            {
                defaultUnityLogHandler = Debug.unityLogger.logHandler;
            }

            var instanceWasNull = instance == null;

            instance = target != null ? new UnityLogHandler(target) : new EmptyHandler();
            Debug.unityLogger.logHandler = instance;

            var instanceHasChanged = instanceWasNull != (instance == null);
            if (instanceHasChanged)
            {
                LogCategory.UpdateAllLoggers();
            }
        }

        /// <summary>
        /// Reverts Unitys logger to the state that it was in before <see cref="CreateAndAssign(LogCategory)"/> was first called.
        /// </summary>
        public static void ResetToDefault()
        {
            if (instance == null) return;

            instance = null;

            Debug.unityLogger.logHandler = defaultUnityLogHandler;

            LogCategory.UpdateAllLoggers();
        }

        /// <summary>
        /// Creates a new <see cref="UnityLogHandler"/> that forwards logs to <paramref name="target"/>.
        /// </summary>
        private UnityLogHandler(LogCategory target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            this.target = target;
        }

        void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
        {
            target.Error?.Log(exception);
        }

        void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    target.Error?.Log(string.Format(format, args));
                    break;
                case LogType.Warning:
                    target.Warning?.Log(string.Format(format, args));
                    break;
                case LogType.Log:
                    target.Info?.Log(string.Format(format, args));
                    break;
                case LogType.Assert:
                    target.Debug?.Log(string.Format(format, args));
                    break;
            }
        }
    }
}
