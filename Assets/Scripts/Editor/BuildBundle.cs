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

    [MenuItem("Window/删除上次打包")]
    private static void ClearAssetBundleSet()
    {
        EditorUtility.DisplayProgressBar("删除AB包名字", "", 0);
        Array.ForEach(AssetDatabase.GetAllAssetBundleNames(),
            abName => AssetDatabase.RemoveAssetBundleName(abName, true));
        // var dir = new DirectoryInfo(Application.streamingAssetsPath).GetFileSystemInfos();
        // foreach (var i in dir)
        // {
        //     if (i is DirectoryInfo)
        //     {
        //         var subDir = new DirectoryInfo(i.FullName);
        //         subDir.Delete(true);
        //     }
        //     else
        //     {
        //         File.Delete(i.FullName);
        //     }
        // }

        EditorUtility.ClearProgressBar();
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

        if (!GUILayout.Button("检查依赖并设置bundle", GUILayout.Height(30)))
        {
            return;
        }
        
        ClearAssetBundleSet();
        var config = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(_path);
        if (!config)
        {
            throw new ArgumentException();
        }

        var save = new Dictionary<string, HashSet<string>>();
        foreach (var bc in config.bundles) //遍历所有AB设置
        {
            var paths = new HashSet<string>();
            foreach (var p in bc.prefabs) //遍历本AB设置的prefab
            {
                EditorUtility.DisplayProgressBar("查找Prefabs", $"Prefab:{p.name}", 0);
                var path = AssetDatabase.GetAssetPath(p);
                var depends = AssetDatabase.GetDependencies(path);
                foreach (var depend in depends)
                {
                    if (depend.EndsWith(".cs")) //不添加脚本
                    {
                        continue;
                    }

                    paths.Add(depend); //重复资源就不添加
                }
            }

            save.Add(bc.bundleName, paths);
        }

        EditorUtility.DisplayProgressBar("分析冗余", "", 50);
        var publicRes = new HashSet<string>(); //重复资源放入公共公共包
        var only = new HashSet<string>(); //保证资源唯一
        var finalDict = new Dictionary<string, List<string>>(); //最终AB资源表
        foreach (var pair in save)
        {
            var abName = pair.Key;
            var assets = pair.Value;
            foreach (var asset in assets)
            {
                if (!only.Add(asset))
                {
                    Debug.LogWarning($"冗余资源 {abName}:{asset}");
                    publicRes.Add(asset);
                }
                else
                {
                    if (finalDict.TryGetValue(abName, out var assList))
                    {
                        assList.Add(asset);
                    }
                    else
                    {
                        finalDict.Add(abName, new List<string> {asset});
                    }
                }
            }
        }

        void SetName(string abName, IEnumerable<string> assAddrColl)
        {
            foreach (var assetImporter in assAddrColl.Select(AssetImporter.GetAtPath))
            {
                if (assetImporter == null)
                {
                    throw new ArgumentException();
                }

                if (abName.Contains("."))
                {
                    assetImporter.assetBundleName = abName;
                }
                else
                {
                    assetImporter.assetBundleName = abName + ".bundle";
                }
            }
        }

        EditorUtility.DisplayProgressBar("设置名字", "", 99);
        foreach (var pair in finalDict)
        {
            SetName(pair.Key, pair.Value);
        }

        SetName("publicResource", publicRes);

        EditorUtility.ClearProgressBar();
    }
}