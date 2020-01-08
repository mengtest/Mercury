using UnityEngine;

public class StepEnter : MonoBehaviour
{
	public GameObject step;

	private Collider2D _stepCollider2D;

	private void Start()
	{
		_stepCollider2D = step.GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			Physics2D.IgnoreCollision(_stepCollider2D, collision, false);
		}
	}
}
