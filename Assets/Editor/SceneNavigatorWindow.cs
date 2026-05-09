using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RottenNoble.Editor
{
    public class SceneNavigatorWindow : EditorWindow
    {
        // ── 상수 ──────────────────────────────────────────────────
        private const string BOOTSTRAP_SCENE_NAME = "BootStrapper";
        private const string TEST_SCENE_NAME      = "TestScene";

        // ── 스타일 캐시 ───────────────────────────────────────────
        private GUIStyle _headerStyle;
        private GUIStyle _pathStyle;
        private bool     _stylesInitialized;

        // ── 색상 ──────────────────────────────────────────────────
        private static readonly Color ColorBootstrapBg   = new Color(0.14f, 0.18f, 0.26f, 1f);
        private static readonly Color ColorTestSceneBg   = new Color(0.22f, 0.22f, 0.22f, 1f);
        private static readonly Color ColorNormalBg      = new Color(0.20f, 0.20f, 0.20f, 1f);
        private static readonly Color ColorNormalBgAlt   = new Color(0.18f, 0.18f, 0.18f, 1f);
        private static readonly Color ColorBootstrapText = new Color(0.5f,  0.75f, 1.0f,  1f);
        private static readonly Color ColorAccent        = new Color(0.5f,  0.75f, 1.0f,  1f);
        private static readonly Color ColorInactiveText  = new Color(0.55f, 0.55f, 0.55f, 1f);
        private static readonly Color ColorActiveText    = new Color(1.0f,  1.0f,  1.0f,  1f);
        private static readonly Color ColorPathText      = new Color(0.45f, 0.45f, 0.45f, 1f);
        private static readonly Color ColorWarningBg     = new Color(0.30f, 0.22f, 0.10f, 1f);
        private static readonly Color ColorWarningText   = new Color(1.0f,  0.75f, 0.2f,  1f);

        // ── 스크롤 ────────────────────────────────────────────────
        private Vector2 _scrollPos;

        // ── 미등록 씬 섹션 접기/펼치기 ───────────────────────────
        private bool _showUnregistered = true;

        // ────────────────────────────────────────────────────────
        [MenuItem("RottenNoble/Scene Navigator")]
        public static void Open()
        {
            var window = GetWindow<SceneNavigatorWindow>("Scene Navigator");
            window.minSize = new Vector2(340f, 200f);
            window.Show();
        }

        // ────────────────────────────────────────────────────────
        private void OnGUI()
        {
            InitStyles();

            var registered   = CollectScenes();
            var unregistered = CollectUnregisteredScenes(registered);

            DrawHeader(registered.Count);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // ── 등록된 씬 ─────────────────────────────────────────
            var allScenes = EditorBuildSettings.scenes
                .Where(s => !string.IsNullOrEmpty(s.path))
                .ToList();
            var buildIndexMap = allScenes
                .Select((s, i) => (s.path, i))
                .ToDictionary(t => t.path, t => t.i);

            for (int i = 0; i < registered.Count; i++)
            {
                buildIndexMap.TryGetValue(registered[i].path, out int buildIndex);
                DrawSceneRow(registered[i], displayIndex: i, buildIndex: buildIndex);
            }

            // ── 미등록 씬 경고 섹션 ───────────────────────────────
            if (unregistered.Count > 0)
                DrawUnregisteredSection(unregistered);

            EditorGUILayout.EndScrollView();

            DrawFooter();
        }

        // ── 씬 수집 ───────────────────────────────────────────────
        private List<EditorBuildSettingsScene> CollectScenes()
        {
            var all = EditorBuildSettings.scenes
                .Where(s => !string.IsNullOrEmpty(s.path))
                .ToList();

            // Bootstrap 최상단 고정
            var bootstrap = all.FirstOrDefault(s =>
                System.IO.Path.GetFileNameWithoutExtension(s.path)
                    .Equals(BOOTSTRAP_SCENE_NAME, System.StringComparison.OrdinalIgnoreCase));

            if (bootstrap != null)
            {
                all.Remove(bootstrap);
                all.Insert(0, bootstrap);
            }

            // TestScene Bootstrap 바로 아래 고정
            var testScene = all.FirstOrDefault(s =>
                System.IO.Path.GetFileNameWithoutExtension(s.path)
                    .Equals(TEST_SCENE_NAME, System.StringComparison.OrdinalIgnoreCase));

            if (testScene != null)
            {
                all.Remove(testScene);
                all.Insert(bootstrap != null ? 1 : 0, testScene);
            }

            return all;
        }

        // ── 미등록 씬 수집 ────────────────────────────────────────
        private List<string> CollectUnregisteredScenes(List<EditorBuildSettingsScene> registered)
        {
            var registeredPaths = new HashSet<string>(registered.Select(s => s.path));

            return AssetDatabase.FindAssets("t:SceneAsset")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !registeredPaths.Contains(path))
                .OrderBy(path => path)
                .ToList();
        }

        // ── 헤더 ──────────────────────────────────────────────────
        private void DrawHeader(int count)
        {
            EditorGUILayout.Space(6f);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);
                GUILayout.Label($"Build Scenes  ({count})", _headerStyle);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("⟳ Refresh", GUILayout.Width(72f), GUILayout.Height(20f)))
                    Repaint();

                if (GUILayout.Button("Build Settings", GUILayout.Width(90f), GUILayout.Height(20f)))
                    EditorWindow.GetWindow(typeof(BuildPlayerWindow));

                GUILayout.Space(8f);
            }
            EditorGUILayout.Space(4f);
            DrawSeparator(ColorAccent * 0.6f);
        }

        // ── 씬 행 ─────────────────────────────────────────────────
        private void DrawSceneRow(EditorBuildSettingsScene scene, int displayIndex, int buildIndex)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            bool isBootstrap = sceneName.Equals(BOOTSTRAP_SCENE_NAME, System.StringComparison.OrdinalIgnoreCase);
            bool isTestScene = sceneName.Equals(TEST_SCENE_NAME,      System.StringComparison.OrdinalIgnoreCase);
            bool isActive    = IsSceneCurrentlyOpen(scene.path);

            Color rowColor = isBootstrap ? ColorBootstrapBg
                           : isTestScene ? ColorTestSceneBg
                           : (displayIndex % 2 == 0 ? ColorNormalBg : ColorNormalBgAlt);

            var rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(46f));
            EditorGUI.DrawRect(rowRect, rowColor);
            GUILayout.Space(10f);

            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Space(6f);

                using (new EditorGUILayout.HorizontalScope())
                {
                    // 배지 — 실제 Build Settings 인덱스 표시
                    string badge      = isBootstrap ? "★" : buildIndex.ToString();
                    Color  badgeColor = isBootstrap ? ColorBootstrapText
                                      : new Color(0.45f, 0.45f, 0.45f, 1f);
                    var badgeStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        fontSize  = 10,
                        alignment = TextAnchor.MiddleCenter,
                        normal    = { textColor = badgeColor }
                    };
                    GUILayout.Label(badge, badgeStyle, GUILayout.Width(20f));
                    GUILayout.Space(4f);

                    // 씬 이름
                    Color nameColor = isActive    ? ColorActiveText
                                    : isBootstrap ? ColorBootstrapText
                                    :               ColorInactiveText;
                    var nameStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        fontSize = 12,
                        normal   = { textColor = nameColor }
                    };
                    GUILayout.Label(sceneName + (isActive ? "  ●" : ""), nameStyle);
                }

                GUILayout.Label(scene.path, _pathStyle);
                GUILayout.Space(4f);
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.VerticalScope(GUILayout.Width(110f)))
            {
                GUILayout.Space(8f);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Ping", GUILayout.Width(40f), GUILayout.Height(28f)))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                        if (asset != null)
                        {
                            EditorGUIUtility.PingObject(asset);
                            Selection.activeObject = asset;
                        }
                    }

                    GUILayout.Space(2f);

                    using (new EditorGUI.DisabledScope(isActive))
                    {
                        if (GUILayout.Button("Open", GUILayout.Width(50f), GUILayout.Height(28f)))
                            OpenScene(scene.path);
                    }
                }
            }

            GUILayout.Space(8f);
            EditorGUILayout.EndHorizontal();
            DrawSeparator(new Color(0.13f, 0.13f, 0.13f, 1f));
        }

        // ── 미등록 씬 경고 섹션 ───────────────────────────────────
        private void DrawUnregisteredSection(List<string> paths)
        {
            EditorGUILayout.Space(6f);

            var headerRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(26f));
            EditorGUI.DrawRect(headerRect, new Color(0.25f, 0.18f, 0.08f, 1f));
            GUILayout.Space(10f);

            var warningStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11,
                normal   = { textColor = ColorWarningText }
            };
            GUILayout.Label($"⚠  Build Settings 미등록 씬  ({paths.Count})", warningStyle);
            GUILayout.FlexibleSpace();

            var foldStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = ColorWarningText }
            };
            GUILayout.Label(_showUnregistered ? "▲ 접기" : "▼ 펼치기", foldStyle);
            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown &&
                headerRect.Contains(Event.current.mousePosition))
            {
                _showUnregistered = !_showUnregistered;
                Event.current.Use();
                Repaint();
            }

            DrawSeparator(ColorWarningText * 0.4f);

            if (!_showUnregistered) return;

            foreach (var path in paths)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

                var rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(42f));
                EditorGUI.DrawRect(rowRect, ColorWarningBg);
                GUILayout.Space(10f);

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(5f);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var iconStyle = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = 11,
                            normal   = { textColor = ColorWarningText }
                        };
                        var content = new GUIContent(
                            $"⚠  {sceneName}",
                            "Build Settings에 등록되지 않은 씬입니다.\n빌드에 포함하려면 추가가 필요합니다."
                        );
                        GUILayout.Label(content, iconStyle);
                    }

                    GUILayout.Label(path, _pathStyle);
                    GUILayout.Space(4f);
                }

                GUILayout.FlexibleSpace();

                using (new EditorGUILayout.VerticalScope(GUILayout.Width(140f)))
                {
                    GUILayout.Space(7f);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(new GUIContent("Ping", "Project 창에서 씬 에셋 하이라이트"),
                            GUILayout.Width(40f), GUILayout.Height(26f)))
                        {
                            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                            if (asset != null)
                            {
                                EditorGUIUtility.PingObject(asset);
                                Selection.activeObject = asset;
                            }
                        }

                        GUILayout.Space(2f);

                        bool isActive = IsSceneCurrentlyOpen(path);
                        using (new EditorGUI.DisabledScope(isActive))
                        {
                            if (GUILayout.Button(new GUIContent("Open", "씬 열기"),
                                GUILayout.Width(44f), GUILayout.Height(26f)))
                                OpenScene(path);
                        }

                        GUILayout.Space(2f);

                        if (GUILayout.Button(new GUIContent("+ Build", "Build Settings에 추가"),
                            GUILayout.Width(52f), GUILayout.Height(26f)))
                            AddSceneToBuildSettings(path);
                    }
                }

                GUILayout.Space(8f);
                EditorGUILayout.EndHorizontal();
                DrawSeparator(new Color(0.15f, 0.12f, 0.05f, 1f));
            }
        }

        // ── Build Settings에 씬 추가 ─────────────────────────────
        private void AddSceneToBuildSettings(string path)
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Any(s => s.path == path)) return;

            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Repaint();
        }

        // ── 푸터 ──────────────────────────────────────────────────
        private void DrawFooter()
        {
            EditorGUILayout.Space(4f);
            DrawSeparator(ColorAccent * 0.3f);
            EditorGUILayout.Space(2f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10f);
                var style = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = ColorPathText }
                };
                GUILayout.Label("● = 현재 열린 씬   ★ = BootStrapper   ⚠ = Build Settings 미등록", style);
            }
            EditorGUILayout.Space(4f);
        }

        // ── 씬 열기 ───────────────────────────────────────────────
        private void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }

        // ── 현재 열린 씬 여부 ──────────────────────────────────────
        private bool IsSceneCurrentlyOpen(string path)
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var s = EditorSceneManager.GetSceneAt(i);
                if (s.path == path) return true;
            }
            return false;
        }

        // ── 구분선 ────────────────────────────────────────────────
        private void DrawSeparator(Color color)
        {
            var rect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, color);
        }

        // ── 스타일 초기화 ─────────────────────────────────────────
        private void InitStyles()
        {
            if (_stylesInitialized) return;

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 13,
                normal   = { textColor = Color.white }
            };

            _pathStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal   = { textColor = ColorPathText },
                wordWrap = false
            };

            _stylesInitialized = true;
        }
    }
}
