using System.Linq;
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

    public static float AnimClipLength(this Animator animator, string clipName)
    {
        return animator.runtimeAnimatorController.animationClips.First(clip => clip.name == clipName).length;
    }
}