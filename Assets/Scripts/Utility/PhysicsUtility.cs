using Unity.Mathematics;
using UnityEngine;

public static class PhysicsUtility
{
	public static RaycastHit? Raycast(float3 pos, float3 dir, float length)
	{
		return Physics.Raycast(pos, math.normalize(dir), out var hit, length) ? new RaycastHit?(hit) : null;
	}

	/// <summary>
	/// 2D射线碰撞检测
	/// </summary>
	/// <param name="pos">开始位置</param>
	/// <param name="dir">射线方向</param>
	/// <param name="length">射线长度</param>
	/// <returns>如果碰撞则返回碰撞结果，否则返回null</returns>
	public static RaycastHit2D? Raycast2D(float2 pos, float2 dir, float length, int layer)
	{
		var nor = math.normalize(dir);
		var hit = Physics2D.Raycast(pos, nor, length, layer);
		Debug.DrawRay(new Vector3(pos.x, pos.y, -1), new Vector3(nor.x, nor.y, 0), Color.red);
		return hit.collider == null ? null : new RaycastHit2D?(hit);
	}

	/// <summary>
	/// 检测墙
	/// </summary>
	/// <param name="pos">开始位置</param>
	/// <param name="dir">射线方向</param>
	/// <param name="length">射线长度</param>
	/// <returns>0：没撞到任何东西，1：实心墙，2：可穿越墙</returns>
	public static int HitWall2D(float2 pos, float2 dir, float length)
	{
		var hit = Raycast2D(pos, dir, length, LayerMask.NameToLayer("Step"));
		if (!hit.HasValue)
		{
			return 0;
		}
		Debug.Log(hit.Value.transform.name);
		return hit.Value.transform.GetComponent<Step>().canThrough ? 2 : 1;
	}

	/// <summary>
	/// 修正X轴坐标，使得返回的X坐标必定不在墙内。
	/// </summary>
	/// <param name="pos">脚下的坐标</param>
	/// <param name="leg">移动X方向的距离（正数为右，负数为左）</param>
	/// <returns></returns>
	public static float XaxisCCorrection(float2 pos, float leg)
	{
		float2 dir;
		dir = leg > 0 ? new float2(1, 0) : new float2(-1, 0);
		int ret = HitWall2D(pos, dir, math.abs(leg));
		//Debug.Log(pos.x);
		if (ret == 1)
		{
			return pos.x;
		}
		else
		{
			return pos.x + leg;
		}
	}

	/// <summary>
	/// 修正Y轴坐标，使得返回的X坐标必定不在墙内。(如果在墙内并没有越过墙会导致掉落)
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="leg"></param>
	/// <returns></returns>
	public static float YaxisCCorrection(float2 pos, float leg)
	{
		float2 dir;
		dir = leg > 0 ? new float2(0, 1) : new float2(0, -1);
		int ret = HitWall2D(pos, dir, math.abs(leg));
		if (ret == 1 || ret == 2)
			Debug.Log(1);
		if (leg > 0)
		{
			if (ret == 1)
			{
				return pos.y;
			}
			else
			{
				return pos.y + leg;
			}
		}
		else
		{
			if (ret == 1 || ret == 2 && HitWall2D(pos, new float2(0, 1), 0.01f) != 2)
			{
				return pos.y;
			}
			else
			{
				return pos.y + leg;
			}
		}
	}
}
