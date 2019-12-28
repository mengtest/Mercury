using UnityEngine;

public class Player : MonoBehaviour
{
	private Rigidbody2D _rigidbody2D;
	private Collider2D _collider2D;
	private SpriteRenderer _spriteRenderer;

	private bool _isOnStep;
	private Collision2D _standingStep;

	private void Start()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_collider2D = GetComponent<Collider2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			_rigidbody2D.AddForce(Vector2.up * 500);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (_isOnStep)
			{
				Physics2D.IgnoreCollision(_collider2D, _standingStep.collider, true);
			}
		}
		if (Input.GetKey(KeyCode.A))
		{
			_rigidbody2D.AddForce(Vector2.left * 10);
			_spriteRenderer.flipX = false;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			_rigidbody2D.AddForce(Vector2.right * 10);
			_spriteRenderer.flipX = true;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Step"))
		{
			_isOnStep = true;
			_standingStep = collision;
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Step"))
		{
			_isOnStep = true;
			_standingStep = collision;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Step"))
		{
			_isOnStep = false;
			_standingStep = null;
		}
	}
}
