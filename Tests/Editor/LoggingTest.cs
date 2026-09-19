
namespace TeaSpoons.Logging.Editor.Tests
{
    using UnityEngine;
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using Logging.Setup;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;

    [TestMustExpectAllLogs]
    public class LoggingTest
    {
        private readonly struct TestLogData : ILogData
        {
            IEnumerator<(string Key, string Value)> IEnumerable<(string Key, string Value)>.GetEnumerator()
            {
                yield return ("key1", "value1");
                yield return ("key2", "value2");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<(string Key, string Value)>)this).GetEnumerator();
            }
        }

        private string lastMessage;
        private LogCategory lastCategory;
        private ILogData lastLogData;
        private int lastFlags;

        private const string UnitTestLogCategoryName = "UnitTest";
        private readonly LogCategory unitTestLog = new LogCategory(UnitTestLogCategoryName);
        private readonly LogCategory otherUnitTestLog = new LogCategory("Other UnitTest");

        private const string VisibleLogMessage = "Test Case Log";

        [SetUp]
        public void SetUp()
        {
            ResetLoggingSetup();

            LoggingSetup.SetMaxLogLevel(unitTestLog, LogLevel.Debug);
            LoggingSetup.SetMaxLogLevel(otherUnitTestLog, LogLevel.Debug);
        }

        [TearDown]
        public void TearDown()
        {
            ResetLoggingSetup();

            lastMessage = null;
            lastCategory = null;
            lastLogData = null;
            lastFlags = 0;

            LoggingSetup.RemoveCustomLogAction(unitTestLog, LogLevel.Debug, LogToLastMessage);
            LoggingSetup.SetMaxLogLevel(unitTestLog, LogCategory.defaultMaxLogLevel);
        }

        private static void ResetLoggingSetup()
        {
            EditorMaxLogLevelOverrides.Clear();

            UnityLogHandler.ResetToDefault();

            LoggingSetup.SetDefaultLogAction(LogLevel.Trace, null);
            LoggingSetup.SetDefaultLogAction(LogLevel.Debug, null);
            LoggingSetup.SetDefaultLogAction(LogLevel.Info, null);
            LoggingSetup.SetDefaultLogAction(LogLevel.Warning, null);
            LoggingSetup.SetDefaultLogAction(LogLevel.Error, null);

            LoggingSetup.SetGlobalMaxLogLevel(LogLevel.Highest);
        }

        [Test]
        public void UseUnityLogAction()
        {
            var expectedRegex = new Regex($"(?s).*{UnitTestLogCategoryName}.*{VisibleLogMessage}.*");

            LogAssert.Expect(LogType.Log, expectedRegex);
            unitTestLog.Info?.Log(VisibleLogMessage);

            LogAssert.Expect(LogType.Warning, expectedRegex);
            unitTestLog.Warning?.Log(VisibleLogMessage);
        }

        [Test]
        public void UseDefaultLogAction()
        {
            LoggingSetup.SetDefaultLogAction(LogLevel.Info, LogToLastMessage);

            var message = "Something happened";

            unitTestLog.Info?.Log(message);

            Assert.AreEqual(message, lastMessage);
            Assert.AreSame(unitTestLog, lastCategory);

            message = VisibleLogMessage;

            otherUnitTestLog.Info?.Log(message);

            Assert.AreEqual(message, lastMessage);
            Assert.AreSame(otherUnitTestLog, lastCategory);
        }

        [Test]
        public void UseCustomLogAction()
        {
            UnityLogHandler.CreateAndAssign(null);
            LoggingSetup.AddCustomLogAction(unitTestLog, LogLevel.Debug, LogToLastMessage);

            var message = "Something happened";

            unitTestLog.Debug?.Log(message);

            Assert.AreEqual(message, lastMessage);
            Assert.AreSame(unitTestLog, lastCategory);

            var ignoredMessage = "Something happened again";

            otherUnitTestLog.Debug?.Log(ignoredMessage);

            Assert.AreEqual(message, lastMessage);
            Assert.AreSame(unitTestLog, lastCategory);
        }

        [Test]
        public void ShortCircuitWhenNoLogAction()
        {
            UnityLogHandler.CreateAndAssign(null);

            Assert.IsNull(otherUnitTestLog.Debug);

            string SkipThis()
            {
                Assert.Fail("The SkipThis method has been called, but it should have been short-circuited.");
                return "Oh no";
            }

            otherUnitTestLog.Debug?.Log(SkipThis());

            Assert.AreEqual(null, lastMessage);
            Assert.AreSame(null, lastCategory);
        }

        [Test]
        public void ChangeMaxLogLevel()
        {
            LoggingSetup.SetDefaultLogAction(LogLevel.Debug, LogToLastMessage);
            LoggingSetup.SetDefaultLogAction(LogLevel.Error, LogToLastMessage);

            var message = "Something happened";
            unitTestLog.Debug?.Log(message);
            Assert.AreEqual(message, lastMessage);

            LoggingSetup.SetMaxLogLevel(unitTestLog, LogLevel.Info);

            var ignoredMessage = "Something happened again";
            unitTestLog.Debug?.Log(ignoredMessage);
            Assert.AreEqual(message, lastMessage);

            // Let's not have errors in the console...
            UnityLogHandler.CreateAndAssign(null);

            message = "An error happened";
            unitTestLog.Error?.Log(message);
            Assert.AreEqual(message, lastMessage);

            LoggingSetup.MuteLogCategory(unitTestLog);

            unitTestLog.Error?.Log(ignoredMessage);
            Assert.AreEqual(message, lastMessage);

            LoggingSetup.SetMaxLogLevel(unitTestLog, LogLevel.Error);

            message = "A new error happened";
            unitTestLog.Error?.Log(message);
            Assert.AreEqual(message, lastMessage);
        }

        [Test]
        public void ChangeGlobalMaxLogLevel()
        {
            LoggingSetup.SetDefaultLogAction(LogLevel.Debug, LogToLastMessage);
            LoggingSetup.SetDefaultLogAction(LogLevel.Error, LogToLastMessage);

            var message = "Something happened";
            unitTestLog.Debug?.Log(message);
            Assert.AreEqual(message, lastMessage);

            LoggingSetup.SetGlobalMaxLogLevel(LogLevel.Info);

            var ignoredMessage = "Something happened again";
            unitTestLog.Debug?.Log(ignoredMessage);
            Assert.AreEqual(message, lastMessage);

            // Let's not have errors in the console...
            UnityLogHandler.CreateAndAssign(null);

            message = "An error happened";
            unitTestLog.Error?.Log(message);
            Assert.AreEqual(message, lastMessage);

            LoggingSetup.MuteLogging();

            unitTestLog.Error?.Log(ignoredMessage);
            Assert.AreEqual(message, lastMessage);

            LoggingSetup.SetGlobalMaxLogLevel(LogLevel.Error);

            message = "A new error happened";
            unitTestLog.Error?.Log(message);
            Assert.AreEqual(message, lastMessage);
        }

        [Test]
        public void HandleUnityLogs()
        {
            LoggingSetup.AddCustomLogAction(unitTestLog, LogLevel.Warning, LogToLastMessage);

            UnityLogHandler.CreateAndAssign(unitTestLog);

            var message = "Debug.LogWarning message";

            Debug.LogWarning(message);

            Assert.AreEqual(message, lastMessage);
        }


        [Test]
        public void UseAdditionalData()
        {
            LoggingSetup.SetDefaultLogAction(LogLevel.Info, LogToLastMessage);

            var logData = new TestLogData();
            unitTestLog.Info?.Log(string.Empty, logData);

            Assert.AreEqual(logData.Count(), lastLogData.Count());
            foreach (var item in logData.Zip(lastLogData, (left, right) => (left, right)))
            {
                Assert.AreEqual(item.left.Key, item.right.Key);
                Assert.AreEqual(item.left.Value, item.right.Value);
            }
        }

        [Test]
        public void UseFlags()
        {
            LoggingSetup.SetDefaultLogAction(LogLevel.Info, LogToLastMessage);

            unitTestLog.Info?.Log(string.Empty, 42);

            Assert.AreEqual(42, lastFlags);
        }

        private void LogToLastMessage(in LogEntry logEntry)
        {
            lastMessage = logEntry.Message;
            lastCategory = logEntry.Category;
            lastLogData = logEntry.AdditionalData;
            lastFlags = logEntry.Flags;
        }
    }
}
