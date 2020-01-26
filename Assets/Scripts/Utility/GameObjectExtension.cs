using UnityEngine;

public static class GameObjectExtension
{
	public static GameObject Show(this GameObject gameObject)
	{
		gameObject.SetActive(true);
		return gameObject;
	}

	public static GameObject Hide(this GameObject gameObject)
	{
		gameObject.SetActive(false);
		return gameObject;
	}
}
