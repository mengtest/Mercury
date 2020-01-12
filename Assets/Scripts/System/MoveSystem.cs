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
		var sprite = entity.GetComponent<SpriteRenderer>();
		var stateable = entity as ISkillable;
		if (stateable.FSMSystem.CurrentState.GetType() != typeof(NormalState))
		{
			return;
		}

		var rayStartPos = new float2(entity.transform.position.x, entity.transform.position.y - 0.5f);//因为position获取的是玩家的中心点，所以减去一段距离才是玩家脚部
		var entityZ = entity.transform.position.z;
		RaycastHit2D? under = PhysicsUtility.Raycast2D(rayStartPos, new float2(0, -1), 5);//返回的是一个可为null的值
		Debug.DrawRay(new Vector3(rayStartPos.x, rayStartPos.y, entityZ), new Vector3(0, -5, entityZ), Color.red);//可以在Scene下看到红色射线，Game看不到，debug用
		//射线碰到第一个物体后就会直接返回一个值，不会继续进行碰撞
		if (under.HasValue)//检查是否有碰撞结果，是的话进if
		{
			RaycastHit2D v = under.Value;//获取碰撞结果
			if (v.transform.CompareTag("Step"))//如果是台阶
			{
				return;//举个例子，不移动了
			}
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




		/*
if (Input.GetKeyDown(KeyCode.S))
{
	if (state.isOnStep && state.standedStep.gameObject.GetComponent<Step>().canThrough)
	{
		Physics2D.IgnoreCollision(colli, state.standedStep.collider, true);
	}
}*/

		entity.transform.position += new Vector3(move.nowSpeedX * Time.deltaTime,0, 0);
		entity.transform.position += new Vector3(0, move.nowSpeedY * Time.deltaTime, 0);
		//Debug.Log(move.nowSpeedY);
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
				move.nowSpeedY = 5;
				move.doubleJumpReady = true;
				move.doubleJumpColdDownTime = 0.2f;
				state.isOnStep = false;
			}
			else if (move.doubleJumpColdDownTime <= 0 && move.doubleJumpReady)
			{
				//rigid.velocity = new Vector2(0, move.maxSpeed * 0.45f);
				move.nowSpeedY = 5;
				move.doubleJumpReady = false;
			}
		}

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
		if (state.isOnStep)
		{
			if (move.nowSpeedX > 0)
			{
				move.nowSpeedX = move.nowSpeedX - 1 < 0 ? 0 : move.nowSpeedX - 1;
			}
			else
			{
				move.nowSpeedX = move.nowSpeedX + 1 > 0 ? 0 : move.nowSpeedX + 1;
			}
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
