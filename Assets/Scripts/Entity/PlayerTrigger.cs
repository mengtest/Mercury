using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
	public EntityPlayer player;
	private Collider2D _playerCollider;

	private void Start()
	{
		_playerCollider = player.GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Step"))
		{
			Physics2D.IgnoreCollision(_playerCollider, other, true);
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Step"))
		{
			Physics2D.IgnoreCollision(_playerCollider, other, true);
		}
	}
}
