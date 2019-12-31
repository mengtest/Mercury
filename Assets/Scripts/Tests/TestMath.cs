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
			//var res = MathUtility.AngleToRadian(45);
			var a = MathUtility.GetRadianFromPosWithXAxis2D(new Vector3(0, 0), new Vector3(-1, -1));
			Assert.AreEqual(MathUtility.RadianToAngle(a), 180+45);
		}
	}
}
