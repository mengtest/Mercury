using UnityEngine;

public class Player : MonoBehaviour
{
	private Rigidbody2D _rigidbody2D;
	private Collider2D _collider2D;
	private SpriteRenderer _spriteRenderer;

	private bool _isOnStep;
	private Collision2D _standingStep;
	private bool _doubleJumpReady;

	private float _moveSpeed = 2;
	private float _nowSpeed = 0;
	private float _maxSpeed = 15;
	private float _jumpSpeed = 0;

	private float _doubleJumpColdDownTime = 0;

	private void Start()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_collider2D = GetComponent<Collider2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		transform.position += new Vector3(_nowSpeed * Time.deltaTime,0,0);
		transform.position += new Vector3(0, _jumpSpeed * Time.deltaTime, 0);
		if (_doubleJumpColdDownTime > 0) _doubleJumpColdDownTime -= Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.W))
		{
			if (_isOnStep)
			{
				_rigidbody2D.AddForce(Vector2.up * 500);
				//_jumpSpeed = 15;
				_doubleJumpReady = true;
				_doubleJumpColdDownTime = 0.3f;
				_isOnStep = false;
			}
			else if (_doubleJumpColdDownTime <= 0 && _doubleJumpReady)
			{
				_rigidbody2D.velocity = new Vector2(0, _maxSpeed*0.6f);
				//_rigidbody2D.AddForce(Vector2.up * 500);
				//if (_jumpSpeed + 15 > 15) _jumpSpeed += 15;
				//else _jumpSpeed = 30f;
				_doubleJumpReady = false;
			}
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
			//_rigidbody2D.AddForce(Vector2.left * 10);
			_nowSpeed -= 3 * _moveSpeed;
			if (_nowSpeed < -2 * _moveSpeed)
				_nowSpeed = -2 * _moveSpeed;
			_spriteRenderer.flipX = false;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			//_rigidbody2D.AddForce(Vector2.right * 10);
			_nowSpeed += 3 * _moveSpeed;
			if (_nowSpeed > 2 * _moveSpeed)
				_nowSpeed = 2 * _moveSpeed;
			_spriteRenderer.flipX = true;
		}
		if (_nowSpeed > 0) _nowSpeed = (_nowSpeed - 1 < 0 ? 0 : _nowSpeed - 1);
		else _nowSpeed = (_nowSpeed + 1 > 0 ? 0 : _nowSpeed + 1);
		if (_jumpSpeed > 0) _jumpSpeed = (_jumpSpeed - 1 < 0 ? 0 : _jumpSpeed - 1);
		if (_rigidbody2D.velocity.magnitude > _maxSpeed)
		{
			_rigidbody2D.velocity = new Vector2(0, -_maxSpeed);
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
