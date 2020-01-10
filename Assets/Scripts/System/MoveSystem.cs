using Unity.Mathematics;
using UnityEngine;

public class MoveSystem : IEntitySystem
{
	public bool IsPhysic { get; } = false;

	private MoveSystem() { }

	public void OnUpdate(Entity entity)
	{
		var move = entity.GetProperty<MoveCapability>();
		var state = entity.GetProperty<BasicState>();
		var rigid = entity.GetComponent<Rigidbody2D>();
		var colli = entity.GetComponent<Collider2D>();
		//var sprite = entity.GetComponent<SpriteRenderer>();
		var stateable = entity as ISkillable;
		if (stateable.FSMSystem.CurrentState.GetType() != typeof(NormalState))
		{
			return;
		}
		/*
		var velocity = rigid.velocity;
		var maxSpeed = 20;
		var s = maxSpeed - math.abs(velocity.x);
		var y = 15 - math.abs(velocity.y);
		var a = s > 0 ? s : 0;
		if (Input.GetKeyDown(KeyCode.W))
		{
			if (velocity.y <= 0)
			{
				y = 15;
			}
			rigid.AddForce(new float2(0, y), ForceMode2D.Impulse);
		}
		float e;
		if (Input.GetKey(KeyCode.A))
		{
			if (!state.isOnStep)
			{
				e = 0.5f;
			}
			else
			{
				e = 1.25f;
			}
			rigid.AddForce(new float2(-a, 0) * e);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			if (!state.isOnStep)
			{
				e = 0.5f;
			}
			else
			{
				e = 1.25f;
			}
			rigid.AddForce(new float2(a, 0) * e);
		}
		*/

		entity.transform.position += new Vector3(move.nowSpeed * Time.deltaTime, 0, 0);
		if (move.doubleJumpColdDownTime > 0) move.doubleJumpColdDownTime -= Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.W))
		{
			if (state.isOnStep)
			{
				rigid.velocity = new Vector2(0, move.maxSpeed * 0.6f);
				move.doubleJumpReady = true;
				move.doubleJumpColdDownTime = 0.2f;
				state.isOnStep = false;
			}
			else if (move.doubleJumpColdDownTime <= 0 && move.doubleJumpReady)
			{
				rigid.velocity = new Vector2(0, move.maxSpeed * 0.45f);
				move.doubleJumpReady = false;
			}
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (state.isOnStep && state.standedStep.gameObject.GetComponent<Step>().canThrough)
			{
				Physics2D.IgnoreCollision(colli, state.standedStep.collider, true);
			}
		}
		if (Input.GetKey(KeyCode.A))
		{
			move.nowSpeed -= 3 * move.moveSpeed;
			if (move.nowSpeed < -2 * move.moveSpeed)
			{
				move.nowSpeed = -2 * move.moveSpeed;
			}
			var raw = entity.transform.eulerAngles;
			entity.transform.eulerAngles = new Vector3(raw.x, 0, raw.z);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			move.nowSpeed += 3 * move.moveSpeed;
			if (move.nowSpeed > 2 * move.moveSpeed)
			{
				move.nowSpeed = 2 * move.moveSpeed;
			}
			var raw = entity.transform.eulerAngles;
			entity.transform.eulerAngles = new Vector3(raw.x, 180, raw.z);
		}
		if (move.nowSpeed > 0)
		{
			move.nowSpeed = move.nowSpeed - 1 < 0 ? 0 : move.nowSpeed - 1;
		}
		else
		{
			move.nowSpeed = move.nowSpeed + 1 > 0 ? 0 : move.nowSpeed + 1;
		}
		if (move.jumpSpeed > 0)
		{
			move.jumpSpeed = move.jumpSpeed - 1 < 0 ? 0 : move.jumpSpeed - 1;
		}
		if (rigid.velocity.magnitude > move.maxSpeed)
		{
			rigid.velocity = new Vector2(0, -move.maxSpeed);
		}
	}
}
