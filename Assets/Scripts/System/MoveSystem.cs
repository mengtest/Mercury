using Unity.Mathematics;
using UnityEngine;

public class MoveSystem : IEntitySystem
{
	public bool IsPhysic { get; } = false;

	private MoveSystem() { }

	public void OnUpdate(Entity entity)
	{
		var move = entity.GetProperty<MoveCapability>();
		var rigid = entity.GetComponent<Rigidbody2D>();
		var coll = entity.GetComponent<Collider2D>();
		if (!move.canMove)
		{
			return;
		}

		move.UpdateJumpCD();
		var velocity = rigid.velocity;
		var maxSpeed = move.moveSpeed;
		var yMaxSpeed = move.jumpSpeed;
		var forceAddX = maxSpeed - math.abs(velocity.x);
		forceAddX = math.max(forceAddX, 0);
		var forceAddY = yMaxSpeed - math.abs(velocity.y);
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
		{
			if (entity.IsGround(0.25f))
			{
				move.RecoverJumpCount();
			}
			if (velocity.y <= 0)
			{
				forceAddY = yMaxSpeed + (-velocity.y);
			}
			if (move.TryJump())
			{
				rigid.AddForce(new float2(0, forceAddY), ForceMode2D.Impulse);
			}
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (entity.IsGround(0.25f, out var step) && step && step.CompareTag("StepCross"))
			{
				Physics2D.IgnoreCollision(coll, step, true);
			}
		}
		if (Input.GetKey(KeyCode.A))
		{
			Move(entity, rigid, -forceAddX, Face.Left);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Move(entity, rigid, forceAddX, Face.Right);
		}

		/*
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
		*/
	}

	private static void Move(Entity entity, Rigidbody2D rigid, float forceAdd, Face face)
	{
		var forceCoe = (entity.IsGround(0.25f) ? 1 : 0.2f) * 0.68f;
		rigid.AddForce(new float2(forceAdd, 0) * forceCoe);
		if (entity.GetFace() != face)
		{
			entity.Rotate(face);
		}
	}
}
