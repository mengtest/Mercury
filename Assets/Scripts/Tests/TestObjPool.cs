using NUnit.Framework;
using UnityEngine;

namespace Tests
{
	public class TestObjPool
	{
		[Test]
		public void TestObjPoolSimplePasses()
		{
			var objPool = new ObjectPool<SimpleTest>(() => new SimpleTest(), 10);
			objPool.OnGet += obj => Debug.Log($"获取:{obj}");
			objPool.OnPreRecycle += obj =>
			{
				Debug.Log($"判断回收:{obj}");
				return obj.GetType() == typeof(SimpleTest);
			};
			objPool.OnRecycle += obj => Debug.Log($"回收:{obj}");
			objPool.Factory.OnDestruct += obj => Debug.Log($"析构:{obj}"); ;

			Assert.AreEqual(objPool.Count, 10);
			Assert.AreEqual(objPool.Factory.MakeCount, 10);

			var o0 = objPool.Get();
			Assert.AreEqual(objPool.Count, 9);
			Assert.AreEqual(objPool.Factory.MakeCount, 10);

			objPool.TrimExcess(0);
			Assert.AreEqual(objPool.Count, 0);
			Assert.AreEqual(objPool.Factory.MakeCount, 1);

			objPool.Recycle(o0);
			Assert.AreEqual(objPool.Count, 1);
			Assert.AreEqual(objPool.Factory.MakeCount, 1);
		}

		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		//[UnityTest]
		//public IEnumerator TestObjPoolWithEnumeratorPasses()
		//{
		// Use the Assert class to test conditions.
		// Use yield to skip a frame.
		//    yield return null;
		//}
	}

	class SimpleTest
	{
		//public int val;
	}
}
