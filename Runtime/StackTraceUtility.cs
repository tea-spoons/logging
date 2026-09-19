
namespace TeaSpoons.Logging
{
    using System;
    using System.Reflection;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Functionality for properly handling stack traces without using Unity's C++ implementation.
    /// </summary>
    public static class StackTraceUtility
    {
        private static readonly StringBuilder stringBuilder = new();
        private static string projectFolder;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Initialize()
        {
            projectFolder = FindProjectFolder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetFormattedStackTrace(Exception exception)
        {
            stringBuilder.Clear();
            var stackTrace = exception.StackTrace;

#if UNITY_EDITOR
            var lines = stackTrace.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex];
                var pathIndex = line.Replace('\\', '/').IndexOf(projectFolder);
                if (pathIndex >= 0)
                {
                    // Length of the "[0x00001] in " part that we don't need
                    const int hexCodeLength = 13;
                    stringBuilder.Append(line, 0, pathIndex - hexCodeLength)
                        .Append("in ");

                    var pathStart = pathIndex + projectFolder.Length;
                    var fullPathLength = line.Length - pathStart;

                    var lineNumberIndex = line.LastIndexOf(':') + 1;
                    var lineNumberLength = line.Length - lineNumberIndex;
                    var pathLength = fullPathLength - lineNumberLength - 1;

                    stringBuilder.Append("<a href=\"")
                        .Append(line, pathStart, pathLength)
                        .Append("\" line=\"")
                        .Append(line, lineNumberIndex, lineNumberLength)
                        .Append("\">");
                    stringBuilder.Append(line, pathStart, fullPathLength);
                    stringBuilder.Append("</a>");
                }
                stringBuilder.AppendLine();
            }
            stackTrace = stringBuilder.ToString();
#endif

            return stackTrace;
        }

        private static string FindProjectFolder()
        {
            var field = typeof(UnityEngine.StackTraceUtility).GetField("projectFolder", BindingFlags.NonPublic | BindingFlags.Static);
            return (string)field.GetValue(null);
        }
    }
}
