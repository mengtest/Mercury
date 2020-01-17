using UnityEngine;

public class StepEnter : MonoBehaviour
{
	public GameObject step;

	public Collider2D _stepCollider2D;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			Physics2D.IgnoreCollision(_stepCollider2D, collision, false);
		}
	}
}
