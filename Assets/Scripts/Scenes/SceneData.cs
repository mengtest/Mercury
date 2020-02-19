using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Guirao.UltimateTextDamage;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneData : MonoBehaviour
{
    public UltimateTextDamageManager textDamage;
    public EventSystem eventSystem;
    public bool loaded;

    private void Awake()
    {
        GameManager.Instance.nowScene = this;
        UIManager.Instance.textDamageManager = textDamage;
        if (!eventSystem)
        {
            throw new ArgumentException();
        }

        StartCoroutine(StartLoad());
    }

    private IEnumerator StartLoad()
    {
        while (true)
        {
            if (!loaded && BundleManager.Instance.state == LoadState.Sleep)
            {
                UIManager.Instance.ShowLoadPanel(0);
                AssetManager.ReadyToLoad(() =>
                {
                    foreach (var e in GameManager.Instance.nextSceneEntities)
                    {
                        AssetManager.Instance.LoadedAssets[e.ToString()].Instantiate();
                    }

                    UIManager.Instance.HideLoadPanel();
                    loaded = true;
                    GameManager.Instance.nextSceneEntities = null;
                });
                var entities = GameManager.Instance.nextSceneEntities;
                var regEntry = entities.Select(e => RegisterManager.Instance.Entries[e]);
                var depRes = new HashSet<AssetLocation>();
                foreach (var e in regEntry) //寻找实体依赖的其他注册项
                {
                    if (e.DependAssets == null) //实体依赖的注册项
                    {
                        continue;
                    }

                    foreach (var registryEntry in e.DependAssets)
                    {
                        var entry = RegisterManager.Instance.Entries[registryEntry];
                        if (entry.DependAssets != null) //注册项需要的资源
                        {
                            depRes.UnionWith(entry.DependAssets);
                        }
                    }
                }

                foreach (var res in depRes) //发出加载所有实体依赖的资源的请求
                {
                    AssetManager.AddRequest<UnityEngine.Object>(res);
                }

                foreach (var e in entities) //发出加载实体预制体的请求
                {
                    AssetManager.AddRequest<GameObject>(e);
                }

                AssetManager.StartLoad();
                yield break;
            }

            yield return null;
        }
    }
}