using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class MathUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetRadianBetTwoObjXY180(float3 pivot, float3 pos, out float2 relative)
	{
		var p1f2 = new float2(pivot.x, pivot.y);
		var p2f2 = new float2(pos.x, pos.y);
		relative = p2f2 - p1f2;
		return math.acos(relative.x / math.length(relative));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetRadianBetTwoObjXY360(float3 pivot, float3 pos)
	{
		var res = GetRadianBetTwoObjXY180(pivot, pos, out var relative);
		if (relative.y < 0)
		{
			res = (2 * math.PI) - res;
		}
		return res;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetRadianBetOneObjXY180(float3 pos)
	{
		return math.acos(pos.x / math.length(pos));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetRadianBetOneObjXY360(float3 pos)
	{
		var res = GetRadianBetOneObjXY180(pos);
		if (pos.y < 0)
		{
			res = (2 * math.PI) - res;
		}
		return res;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float2 GetPosInPolarCoord(float2 pivot, float distance, float radian)
	{
		math.sincos(radian, out var sina, out var cosa);
		var vec = new float2(distance * cosa, distance * sina);
		return pivot + vec;
	}
}
