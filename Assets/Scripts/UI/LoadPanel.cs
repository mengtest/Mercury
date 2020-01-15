using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : MonoBehaviour
{
	public Text progressText;
	public Image progressRate;
	private Func<float> _getPercent;

	public float Progress { get; set; } = 0;

	public void Update()
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
