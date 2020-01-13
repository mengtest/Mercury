using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoSingleton<GameManager>
{
	public GameObject pools;
	public GameObject canvas;
	public AssetLabelReference specialEffects;
	public AssetLabelReference entities;
	private readonly Dictionary<string, ObjectPool<GameObject>> _specialEffectPools = new Dictionary<string, ObjectPool<GameObject>>();

	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(pools);
		DontDestroyOnLoad(canvas);
		StartCoroutine(AsyncLoadSpeicalEffect());
		EntitySystemManager.Instance.Init();
	}

	/*
	protected override void Update()
	{
	}
	*/

	public GameObject GetEffect(string key)
	{
		return _specialEffectPools[key].Get();
	}

	public bool RecycleEffect(GameObject gameObject)
	{
		return _specialEffectPools[gameObject.name].Recycle(gameObject);
	}

	public void DestoryEffect(string key)
	{
		var pool = _specialEffectPools[key];
		pool.TrimExcess(0);
	}

	private IEnumerator AsyncLoadSpeicalEffect()
	{
		var poolReq = Addressables.LoadAssetsAsync<GameObject>(specialEffects, null);
		LoadPanel.Instance.Active(() => poolReq.PercentComplete);
		yield return poolReq;
		foreach (var obj in poolReq.Result)
		{
			var go = new GameObject(obj.name + "_Pool");
			go.transform.parent = pools.transform;
			var sePool = new ObjectPool<GameObject>(obj, (o) =>
			{
				var res = Instantiate(o).Hide();
				res.name = o.name;
				res.transform.parent = go.transform;
				return res;
			}, 1);
			sePool.Factory.OnDestruct += ob => Destroy(ob);
			sePool.OnGet += ob => ob.Show();
			sePool.OnRecycle += ob => ob.Hide();
			_specialEffectPools.Add(obj.name, sePool);
		}
		StartCoroutine(AsyncLoadEntities());
	}

	private IEnumerator AsyncLoadEntities()
	{
		var req = Addressables.LoadAssetsAsync<GameObject>(entities, null);
		yield return req;
		foreach (var entity in req.Result)
		{
			Instantiate(entity);
		}
		LoadPanel.Instance.Complete();
	}
}
