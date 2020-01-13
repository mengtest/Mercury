using UnityEngine;

public class SkillObject : MonoBehaviour
{
	public Collider2D Contact { get; private set; }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Contact = collision;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		Contact = collision;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Contact = null;
	}
}
