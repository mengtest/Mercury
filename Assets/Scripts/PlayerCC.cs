using UnityEngine;

public class PlayerCC : MonoBehaviour
{
	public Collider2D Impact { get; private set; }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Impact = collision;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		Impact = collision;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Impact = null;
	}
}
