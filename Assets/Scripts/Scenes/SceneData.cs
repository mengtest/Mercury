using System;
using System.Collections;
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
            if (!loaded && AssetManager.Instance.state == LoadState.Sleep)
            {
                UIManager.Instance.ShowLoadPanel(0);
                AssetManager.Instance.ReadyToLoad(() =>
                {
                    UIManager.Instance.HideLoadPanel();
                    loaded = true;
                    GameManager.Instance.nextSceneElements = null;
                });
                foreach (var element in GameManager.Instance.nextSceneElements)
                {
                    AssetManager.Instance.AddRequest<GameObject>(element,
                        req =>
                        {
                            if (req is AssetBundleRequest abq)
                            {
                                Instantiate(abq.asset);
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        });
                }

                AssetManager.Instance.StartLoad();
                yield break;
            }

            yield return null;
        }
    }
}