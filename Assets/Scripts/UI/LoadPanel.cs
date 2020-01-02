using System;
using UnityEngine.UI;

public class LoadPanel : MonoSingleton<LoadPanel>
{
	public Text progressText;
	public Image progressRate;
	private Func<float> _getPercent;

	public float Progress { get; set; } = 0;

	protected override void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
	}

	protected override void Update()
	{
		Progress = _getPercent();
		progressText.text = Progress * 100 + "%";
		progressRate.fillAmount = Progress;
	}

	public void Active(Func<float> getPercent)
	{
		gameObject.Show();
		Progress = 0;
		_getPercent = getPercent;
	}

	public void Complete()
	{
		gameObject.Hide();
		_getPercent = null;
	}
}
