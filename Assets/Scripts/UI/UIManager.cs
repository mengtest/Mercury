using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
	public LoadPanel loadPanel;
	public GameObject bootstrapPanel;
	public LevelPanel levelPanel;

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
}
