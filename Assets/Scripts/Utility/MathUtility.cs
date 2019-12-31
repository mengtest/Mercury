using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class MathUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float RadianToAngle(float radian)
	{
		return radian * 180 / math.PI;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float AngleToRadian(float angle)
	{
		return angle * math.PI / 180;
	}

	[Obsolete("未完成")]
	public static float GetRadianFromPosWithXAxis2D(float3 pivot, float3 pos)
	{
		var relative = new float2(pivot.x, pivot.y) - new float2(pos.x, pos.y);
		var alpha = relative.y / relative.x;
		var r = math.atan(alpha);
		return alpha < 0 ? math.PI + r : r;
	}
}
