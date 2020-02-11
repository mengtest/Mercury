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
                var depRes = new List<AssetLocation>();
                foreach (var e in regEntry)
                {
                    if (e.DependAssets != null)
                    {
                        depRes.AddRange(e.DependAssets);
                    }
                }

                foreach (var res in depRes)
                {
                    AssetManager.AddRequest<UnityEngine.Object>(res);
                }

                foreach (var e in entities)
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