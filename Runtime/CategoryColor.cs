
#if UNITY_EDITOR
namespace TeaSpoons.Logging
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Helper class that helps deterministically generating a color for each <see cref="LogCategory"/>.
    /// </summary>
    internal static class CategoryColor
    {
        private class Colors
        {
            public readonly Color regular;
            public readonly Color brighter;
            public readonly string hexString;

            public Colors(float hue)
            {
                regular = Color.HSVToRGB(hue, 0.6f, 0.8f);
                brighter = Color.HSVToRGB(hue, 0.5f, 1f);
                hexString = GetHexString(regular);
            }

            public Color Get(bool brighter)
            {
                return brighter ? this.brighter : regular;
            }

            private static string GetHexString(Color32 color)
            {
                return color.r.ToString("X") + color.g.ToString("X") + color.b.ToString("X");
            }
        }

        private static readonly Dictionary<string, Colors> colorCache = new Dictionary<string, Colors>();

        /// <summary>
        /// Returns a color, deterministically generated using the <paramref name="category"/>'s hash code.
        /// </summary>
        /// <param name="brighter">Set to true to make the color slightly brighter to fit brighter backgrounds better.</param>
        public static Color Get(LogCategory category, bool brighter = false)
        {
            return GetOrCreateFor(category).Get(brighter);
        }

        /// <summary>
        /// Returns the hex representation (like <c>#0123AF</c>) of a color, deterministically generated using the <paramref name="category"/>'s hash code.
        /// </summary>
        public static string GetHex(LogCategory category)
        {
            return GetOrCreateFor(category).hexString;
        }

        private static Colors GetOrCreateFor(LogCategory category)
        {
            Colors result;
            if (colorCache.TryGetValue(category.Name, out result))
            {
                return result;
            }

            var categoryHash = category.Name.GetHashCode();
            var hue = Mathf.Abs(categoryHash) / (float)int.MaxValue;
            result = new Colors(hue);
            colorCache.Add(category.Name, result);

            return result;
        }
    }
}
#endif
