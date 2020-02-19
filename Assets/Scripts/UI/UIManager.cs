using System;
using UnityEngine;
using Guirao.UltimateTextDamage;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class UIManager : MonoSingleton<UIManager>
{
    public LoadPanel loadPanel;
    public GameObject bootstrapPanel;
    public LevelPanel levelPanel;
    public GameObject canvas;
    public UltimateTextDamageManager textDamageManager;

    public void Init()
    {
#if UNITY_EDITOR
        if (!canvas)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UIPanel/LoadPanelWithCanvas.prefab");
            canvas = Instantiate(prefab);
            canvas.name = "Canvas";
        }

        if (!loadPanel)
        {
            loadPanel = canvas.GetComponentInChildren<LoadPanel>();
        }

        loadPanel.gameObject.Hide();
        bootstrapPanel?.Show();
        levelPanel?.gameObject.Hide();
#else
        bootstrapPanel.Show();
        levelPanel.gameObject.Hide();
        loadPanel.gameObject.Hide();
#endif
        DontDestroyOnLoad(canvas);
    }
    
    public void OnBootstrapStartBtnEnter()
    {
        levelPanel.gameObject.Show();
        bootstrapPanel.Hide();
    }

    public void ShowDamage(Transform target, float damage, DamageType type)
    {
        textDamageManager.Add(damage.ToString(), target, type.ToString());
    }

    public void ShowLoadPanel(Func<float> getPercent) { loadPanel.Active(getPercent); }

    public void ShowLoadPanel(float progress) { loadPanel.Active(progress); }

    public void HideLoadPanel() { loadPanel.Complete(); }
}