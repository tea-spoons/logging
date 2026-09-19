
namespace TeaSpoons.Logging.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System;
    using System.Text;
    using TeaSpoons.PackageCore;

    /// <summary>
    /// An EditorWindow that allows to forcibly override the max log level of every <see cref="LogCategory"/> declared in the project.
    /// </summary>
    public class LogLevelWindow : EditorWindow
    {
        private readonly struct LogLevelInfo
        {
            public readonly GUIContent ButtonContent;
            public readonly Color Color;

            public LogLevelInfo(GUIContent buttonContent, Color color)
            {
                ButtonContent = buttonContent;
                Color = color;
            }
        }

        private const string editorPrefsKey = "TeaSpoons.Logging.LogLevelWindow.settings";
        private const int groupSize = 4;
        private static readonly Dictionary<LogLevel, LogLevelInfo> logLevelInfos = new Dictionary<LogLevel, LogLevelInfo>();
        private static readonly Dictionary<short, LogLevel> logLevelsByIndex = new Dictionary<short, LogLevel>();
        private static bool isInitialized => logLevelInfos.Count > 0;

        private Vector2 scrollPosition;


        [InitializeOnLoadMethod, InitializeOnEnterPlayMode]
        private static void Initialize()
        {
            InitializeData();
            LoadAndApplySettings();
        }

        [MenuItem(PackageCore.Editor.Menus.RootItem + "Logging/Log Levels")]
        private static void Open()
        {
            var window = GetWindow<LogLevelWindow>();
            window.titleContent = new GUIContent("Log Levels", GetEditorIcon("d_UnityEditor.ConsoleWindow"));
        }

        private void OnGUI()
        {
            if (!isInitialized) return;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            GUILayout.Space(8);
            DisplayCategoryLine(null);
            HorizontalLine();
            DisplayCategories();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private static void InitializeData()
        {
            if (logLevelInfos.Count == 0)
            {
                var lightGray = new Color(0.8f, 0.8f, 0.8f);
                logLevelInfos.Add(LogLevel.None, GetInfoWithIcon("scenevis_hidden", "Mute Category", lightGray));
                logLevelInfos.Add(LogLevel.Error, GetInfoWithIcon("console.erroricon.sml", "Error", new Color(1f, 0.8f, 0.8f)));
                logLevelInfos.Add(LogLevel.Warning, GetInfoWithIcon("console.warnicon.sml", "Warning", new Color(1f, 1f, 0.8f)));
                logLevelInfos.Add(LogLevel.Info, GetInfoWithIcon("console.infoicon.sml", "Info", new Color(0.8f, 0.8f, 1f)));
                logLevelInfos.Add(LogLevel.Debug, new LogLevelInfo(new GUIContent("D", "Debug"), lightGray));
                logLevelInfos.Add(LogLevel.Trace, new LogLevelInfo(new GUIContent("T", "Trace"), lightGray));
            }

            if (logLevelsByIndex.Count == 0)
            {
                logLevelsByIndex.Add(-1, LogLevel.None);
                logLevelsByIndex.Add(0, LogLevel.Error);
                logLevelsByIndex.Add(1, LogLevel.Warning);
                logLevelsByIndex.Add(2, LogLevel.Info);
                logLevelsByIndex.Add(3, LogLevel.Debug);
                logLevelsByIndex.Add(4, LogLevel.Trace);
            }
        }

        private static LogLevelInfo GetInfoWithIcon(string name, string tooltip, Color color)
        {
            return new LogLevelInfo(new GUIContent(GetEditorIcon(name), tooltip), color);
        }

        private static Texture GetEditorIcon(string name)
        {
            return EditorGUIUtility.IconContent(name).image;
        }


        private void DisplayCategories()
        {
            var index = -1;
            foreach (var category in LogCategory.allCategories)
            {
                index++;
                if (index == groupSize)
                {
                    index = 0;
                    HorizontalLine();
                }
                DisplayCategoryLine(category);
            }
        }
        private void DisplayCategoryLine(LogCategory category)
        {
            GUILayout.BeginHorizontal();
            {
                if (category != null)
                {
                    using (GUIColor.Override(CategoryColor.Get(category, EditorGUIUtility.isProSkin)))
                    {
                        GUILayout.Label(category.Name, EditorStyles.boldLabel);
                    }
                }
                else
                {
                    GUILayout.Label("Override all");
                }

                GUILayout.FlexibleSpace();

                DisplayOverrideCheckbox(category);
                DisplayLogLevelButton(category, LogLevel.None);
                DisplayLogLevelButton(category, LogLevel.Error);
                DisplayLogLevelButton(category, LogLevel.Warning);
                DisplayLogLevelButton(category, LogLevel.Info);
                DisplayLogLevelButton(category, LogLevel.Debug);
                DisplayLogLevelButton(category, LogLevel.Trace);
            }
            GUILayout.EndHorizontal();
        }

        private void DisplayLogLevelButton(LogCategory category, LogLevel level)
        {
            var info = logLevelInfos[level];

            var active = category == null || level <= category.effectiveMaxLogLevel;
            using (GUIColor.Override(active ? info.Color : new Color(0.4f, 0.4f, 0.4f)))
            {
                if (GUILayout.Button(info.ButtonContent, GUILayout.Width(34)))
                {
                    if (category != null)
                    {
                        EditorMaxLogLevelOverrides.Set(category.Name, level);
                    }
                    else
                    {
                        foreach (var cat in LogCategory.allCategories)
                        {
                            EditorMaxLogLevelOverrides.Set(cat.Name, level);
                        }
                    }
                    SaveSettings();
                }
            }
        }

        private void DisplayOverrideCheckbox(LogCategory category)
        {
            var wasEnabled = category == null ? EditorMaxLogLevelOverrides.ContainsAny() : EditorMaxLogLevelOverrides.Contains(category.Name);
            GUI.enabled = wasEnabled;
            
            var enabled = GUILayout.Toggle(wasEnabled, GUIContent.none);

            GUI.enabled = true;
            if (wasEnabled && !enabled)
            {
                if (category != null)
                {
                    EditorMaxLogLevelOverrides.Remove(category.Name);
                }
                else
                {
                    EditorMaxLogLevelOverrides.Clear();
                }
                SaveSettings();
            }
        }

        #region Saving and Loading
        /// <summary>
        /// Saves the <see cref="settings"/> array to EditorPrefs.
        /// </summary>
        private void SaveSettings()
        {
            var stringBuilder = new StringBuilder();

            foreach (var setting in EditorMaxLogLevelOverrides.GetAll())
            {
                stringBuilder.Append(setting.Key);
                stringBuilder.Append('=');
                stringBuilder.Append(setting.Value);
                stringBuilder.Append(';');
            }
            PlayerPrefs.SetString(editorPrefsKey, stringBuilder.ToString());
        }

        /// <summary>
        /// Loads the <see cref="settings"/> array from EditorPrefs and applies them.
        /// </summary>
        private static void LoadAndApplySettings()
        {
            EditorMaxLogLevelOverrides.Clear();

            var loadedData = PlayerPrefs.GetString(editorPrefsKey, "").Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var setting in loadedData)
            {
                try
                {
                    var data = setting.Split('=');
                    var categoryName = data[0];
                    var levelIndex = short.Parse(data[1]);
                    EditorMaxLogLevelOverrides.Set(categoryName, logLevelsByIndex[levelIndex]);
                }
                catch { }
            }
        }
        #endregion

        private static void HorizontalLine()
        {
            using (GUIColor.Multiply(Color.gray))
            {
                GUILayout.Box(string.Empty, GUILayout.Height(2), GUILayout.ExpandWidth(true));
                GUILayout.Space(1);
            }
        }
    }
}
