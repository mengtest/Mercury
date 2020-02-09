using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildBundle : EditorWindow
{
    private Rect _rect;
    private string _path;
    private Rect _expRect;
    private string _expPath;

    [MenuItem("Window/导出Bundle")]
    private static void MenuClicker()
    {
        var rect = new Rect(50, 50, 400, 400);
        GetWindowWithRect<BuildBundle>(rect, false, "导出Bundle");
    }

    [MenuItem("Window/删除AssetBundle设置")]
    private static void ClearAssetBundleSet()
    {
        Array.ForEach(AssetDatabase.GetAllAssetBundleNames(),
            abName => AssetDatabase.RemoveAssetBundleName(abName, true));
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("配置路径");
        _rect = EditorGUILayout.GetControlRect(GUILayout.Width(395));
        _path = EditorGUI.TextField(_rect, _path);
        if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) &&
            _rect.Contains(Event.current.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                _path = DragAndDrop.paths[0];
            }
        }

        if (!GUILayout.Button("导出", GUILayout.Height(30)))
        {
            return;
        }

        var config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(_path);
        if (!config)
        {
            throw new ArgumentException();
        }

        foreach (var bc in config.bundles)
        {
            var paths = new HashSet<string>();
            foreach (var p in bc.prefabs)
            {
                EditorUtility.DisplayProgressBar("查找Prefabs", $"Prefab:{p.name}", 0);
                var path = AssetDatabase.GetAssetPath(p);
                var depends = AssetDatabase.GetDependencies(path);
                foreach (var depend in depends)
                {
                    if (depend.EndsWith(".cs"))
                    {
                        continue;
                    }

                    paths.Add(depend);
                }
            }

            foreach (var assetImporter in paths.Select(AssetImporter.GetAtPath))
            {
                EditorUtility.DisplayProgressBar("设置AB包名", $"名字:{assetImporter.assetPath}", 50);
                if (assetImporter == null)
                {
                    throw new ArgumentException();
                }

                assetImporter.assetBundleName = bc.bundleName;
            }
        }

        EditorUtility.ClearProgressBar();

        // var allABs = AssetDatabase.GetAllAssetBundleNames();
        // EditorUtility.DisplayProgressBar("打包中", "", 99);
        // BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,
        //     BuildAssetBundleOptions.ChunkBasedCompression,
        //     BuildTarget.StandaloneWindows64);
    }
}