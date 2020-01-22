using System;
using UnityEngine;
using Guirao.UltimateTextDamage;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class UIManager : MonoSingleton<UIManager>
{
    public GameObject loadPanelPrefab;
    public LoadPanel loadPanel;
    public GameObject bootstrapPanel;
    public LevelPanel levelPanel;
    public UltimateTextDamageManager textDamageManager;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        loadPanelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UIPanel/LoadPanel.prefab");
#endif
        loadPanel = Instantiate(loadPanelPrefab).GetComponent<LoadPanel>();
        loadPanel.transform.SetParent(GameManager.Instance.canvas.transform);
        loadPanel.gameObject.Hide();
#if UNITY_EDITOR
        bootstrapPanel?.Show();
        levelPanel?.gameObject.Hide();
#else
        bootstrapPanel.Show();
        levelPanel.gameObject.Hide();
#endif
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