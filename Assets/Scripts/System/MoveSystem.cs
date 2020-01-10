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

		entity.transform.position += new Vector3(move.nowSpeedX * Time.deltaTime, 0, 0);
		entity.transform.position += new Vector3(0, move.nowSpeedY * Time.deltaTime, 0);
		if (!state.isOnStep)
		{
			move.nowSpeedY = (move.nowSpeedY - 2 < -10) ? (-10) : (move.nowSpeedY - 2);
		}
		if (move.doubleJumpColdDownTime > 0) move.doubleJumpColdDownTime -= Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (state.isOnStep)
			{
				//rigid.velocity = new Vector2(0, move.maxSpeed * 0.55f);
				move.nowSpeedY = 15;
				move.doubleJumpReady = true;
				move.doubleJumpColdDownTime = 0.2f;
				state.isOnStep = false;
			}
			else if (move.doubleJumpColdDownTime <= 0 && move.doubleJumpReady)
			{
				//rigid.velocity = new Vector2(0, move.maxSpeed * 0.45f);
				move.nowSpeedY = 15;
				move.doubleJumpReady = false;
			}
		}
		/*
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (state.isOnStep && state.standedStep.gameObject.GetComponent<Step>().canThrough)
			{
				Physics2D.IgnoreCollision(colli, state.standedStep.collider, true);
			}
		}*/
		if (Input.GetKey(KeyCode.A) & state.isOnStep)
		{
			move.nowSpeedX -= 3 * move.moveSpeed;
			if (move.nowSpeedX < -2 * move.moveSpeed)
			{
				move.nowSpeedX = -2 * move.moveSpeed;
			}
			var raw = entity.transform.eulerAngles;
			entity.transform.eulerAngles = new Vector3(raw.x, 0, raw.z);
		}
		else if (Input.GetKey(KeyCode.D) & state.isOnStep)
		{
			move.nowSpeedX += 3 * move.moveSpeed;
			if (move.nowSpeedX > 2 * move.moveSpeed)
			{
				move.nowSpeedX = 2 * move.moveSpeed;
			}
			var raw = entity.transform.eulerAngles;
			entity.transform.eulerAngles = new Vector3(raw.x, 180, raw.z);
		}
		if (move.nowSpeedX > 0 & state.isOnStep)
		{
			move.nowSpeedX = move.nowSpeedX - 1 < 0 ? 0 : move.nowSpeedX - 1;
		}
		else
		{
			move.nowSpeedX = move.nowSpeedX + 1 > 0 ? 0 : move.nowSpeedX + 1;
		}

		/*
		if (move.jumpSpeed > 0 & state.isOnStep)
		{
			move.jumpSpeed = move.jumpSpeed - 1 < 0 ? 0 : move.jumpSpeed - 1;
		}

		
		if (rigid.velocity.magnitude > move.maxSpeed)
		{
			rigid.velocity = new Vector2(0, -move.maxSpeed);
		}
		*/
	}
}
