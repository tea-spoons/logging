
#if UNITY_EDITOR
namespace TeaSpoons.Logging
{
    using System.Collections.Generic;

    /// <summary>
    /// Max log level overrides that can be enforced through an editor tool.
    /// </summary>
    internal static class EditorMaxLogLevelOverrides
    {
        private static readonly Dictionary<string, LogLevel> overrides = new Dictionary<string, LogLevel>();

        internal static void Set(string key, LogLevel value)
        {
            overrides[key] = value;
        }

        internal static void Remove(string key)
        {
            overrides.Remove(key);
        }

        internal static void Clear()
        {
            overrides.Clear();
        }

        internal static bool Contains(string key)
        {
            return overrides.ContainsKey(key);
        }

        internal static bool ContainsAny()
        {
            return overrides.Count > 0;
        }

        internal static bool TryGet(string key, out LogLevel value)
        {
            return overrides.TryGetValue(key, out value);
        }

        internal static IEnumerable<KeyValuePair<string, LogLevel>> GetAll()
        {
            return overrides;
        }
    }
}
#endif
