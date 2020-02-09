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

    private void Awake() { UIManager.Instance.ShowLoadPanel(0); }

    private void Start()
    {
        GameManager.Instance.nowScene = this;
        UIManager.Instance.textDamageManager = textDamage;
        foreach (var ass in assets)
        {
            ass.InstantiateAsync(null, true);
        }

        UIManager.Instance.HideLoadPanel();
#if UNITY_EDITOR
        
#endif
    }
}