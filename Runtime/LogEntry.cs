
namespace TeaSpoons.Logging
{
    using System;
    using System.Text;

    /// <summary>
    /// A single entry to any log.
    /// Provided to <see cref="Logger.LogAction"/>s so they can use the data as they see fit.
    /// </summary>
    public readonly struct LogEntry
    {
        private static readonly StringBuilder toStringBuilder = new StringBuilder();

        public readonly string Message;
        public readonly Exception Exception;
        public readonly ILogData AdditionalData;
        public readonly LogCategory Category;
        public readonly LogLevel Level;
        public readonly int Flags;

        public bool HasAdditionalData => AdditionalData != null;

        internal LogEntry(string message, Exception exception, in ILogData additionalData, int flags, LogCategory category, LogLevel level)
        {
            Message = message;
            Exception = exception;
            AdditionalData = additionalData;
            Category = category;
            Level = level;
            Flags = flags;
        }

        public bool HasFlag(int flag)
        {
            return (Flags & flag) != 0;
        }

        #region ToString for Default Logging
        /// <summary>
        /// Formats this <see cref="LogEntry"/> for Unity's console.
        /// </summary>
        /// <remarks>
        /// Expensive. Consider a custom solution in builds.
        /// </remarks>
        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string prefix)
        {
            lock (toStringBuilder)
            {
                toStringBuilder.Clear();

                if (!string.IsNullOrEmpty(prefix))
                {
                    toStringBuilder.Append(prefix).Append(" ");
                }

                AppendFormattedCategoryToStringBuilder();
                AppendMessageToStringBuilder();
                AppendAdditionalDataToStringBuilder();
                AppendExceptionToStringBuilder();

                return toStringBuilder.ToString();
            }
        }

        private void AppendFormattedCategoryToStringBuilder()
        {
            if (Category == null) return;

            toStringBuilder.Append(Category.LogPrefix);
        }

        private readonly void AppendMessageToStringBuilder()
        {
            if (Message == null) return;

            toStringBuilder.AppendLine(Message);
        }

        private void AppendExceptionToStringBuilder()
        {
            if (Exception == null) return;

#if UNITY_EDITOR
            toStringBuilder.AppendLine("<b><color=#FF2222>=== Exception ===</color></b>");
#else
            toStringBuilder.AppendLine("=== Exception ===");
#endif
            toStringBuilder.Append(Exception.GetType().Name).Append(": ");
            toStringBuilder.AppendLine(Exception.Message);
            toStringBuilder.Append(StackTraceUtility.GetFormattedStackTrace(Exception));
        }


        private void AppendAdditionalDataToStringBuilder()
        {
            if (AdditionalData == null) return;

#if UNITY_EDITOR
            toStringBuilder.AppendLine("<b><color=#22DDDD>=== Additional Data ===</color></b>");
#else
            toStringBuilder.AppendLine("=== Additional Data ===");
#endif

            foreach (var item in AdditionalData)
            {
                toStringBuilder.AppendLine($"{item.Key}: {item.Value}");
            }
        }
        #endregion
    }
}
