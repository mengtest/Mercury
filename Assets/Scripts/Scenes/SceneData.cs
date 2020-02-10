using System;
using System.Collections.Generic;
using Guirao.UltimateTextDamage;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class SceneData : MonoBehaviour
{
    public List<AssetReference> assets;
    public UltimateTextDamageManager textDamage;
    public EventSystem eventSystem;

    private void Awake()
    {
        var gm = GameManager.Instance;
        UIManager.Instance.ShowLoadPanel(0);
        gm.nowScene = this;
        UIManager.Instance.textDamageManager = textDamage;
    }

    private void Start()
    {
        // foreach (var ass in assets)
        // {
        //     ass.InstantiateAsync(null, true);
        // }
        if (!eventSystem)
        {
            throw new ArgumentException();
        }

        UIManager.Instance.HideLoadPanel();
#if UNITY_EDITOR

#endif
    }
}