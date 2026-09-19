#if !TEASPOONS_PACKAGE_CORE
namespace TeaSpoons.Logging.Editor
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Changes <see cref="GUI.color"/> and reverts when it is disposed.
    /// Stand-in for TeaSpoons.PackageCore.GUIColor, which the window uses instead when package-core is in the project.
    /// </summary>
    internal readonly struct GUIColor : IDisposable
    {
        public static GUIColor Override(Color color) => new GUIColor(color);

        public static GUIColor Multiply(Color color) => new GUIColor(GUI.color * color);

        private readonly Color originalColor;

        private GUIColor(Color color)
        {
            originalColor = GUI.color;
            GUI.color = color;
        }

        void IDisposable.Dispose() => GUI.color = originalColor;
    }
}
#endif
