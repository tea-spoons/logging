namespace TeaSpoons.Logging.Editor.Tests
{
    using System.Reflection;
    using NUnit.Framework;
    using UnityEditor;

    /// <summary>
    /// The menu root comes from package-core when it is installed and from a local fallback when it is not.
    /// Both must give the same menu path.
    /// </summary>
    public class LogLevelWindowMenuTest
    {
        [Test]
        public void LogLevelWindow_MenuItem_IsUnderTeaSpoons()
        {
            var open = typeof(LogLevelWindow).GetMethod("Open", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(open);

            var attribute = open.GetCustomAttribute<MenuItem>();
            Assert.IsNotNull(attribute);
            Assert.AreEqual("TeaSpoons/Logging/Log Levels", attribute.menuItem);
        }
    }
}
