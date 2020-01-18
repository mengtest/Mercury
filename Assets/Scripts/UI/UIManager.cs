using UnityEngine;
using Guirao.UltimateTextDamage;

public class UIManager : MonoSingleton<UIManager>
{
	public LoadPanel loadPanel;
	public GameObject bootstrapPanel;
	public LevelPanel levelPanel;
	public UltimateTextDamageManager textDamageManager;

	protected override void Awake()
	{
		base.Awake();
		loadPanel.gameObject.Hide();
		bootstrapPanel.Show();
		levelPanel.gameObject.Hide();
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
}
