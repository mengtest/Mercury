using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace Tests
{
	public class TestMath
	{
		[Test]
		public void TestMathSimplePasses()
		{
			var a = MathUtility.GetRadianBetTwoObjXY360(new Vector3(0, 0), new Vector3(1, -1));
			Assert.AreEqual((int)math.degrees(a), 315);
			var b = MathUtility.GetRadianBetTwoObjXY360(new Vector3(0, 0), new Vector3(-1, -1));
			Assert.AreEqual((int)math.degrees(b), 225);
			var c = MathUtility.GetRadianBetTwoObjXY360(new Vector3(0, 0), new Vector3(-1, 1));
			Assert.AreEqual((int)math.degrees(c), 135);
			var d = MathUtility.GetRadianBetTwoObjXY360(new Vector3(0, 0), new Vector3(1, 1));
			Assert.AreEqual((int)math.degrees(d), 45);

			var e = MathUtility.GetRadianBetOneObjXY360(new float3(-1, -1, 0));
			Assert.AreEqual((int)math.degrees(e), 225);

			var f = MathUtility.GetPosInPolarCoord(new float2(1, 1), math.sqrt(2), math.PI / 4);
			Assert.AreEqual(f.x, 2);
			Assert.AreEqual(f.y, 2);
		}
	}
}
