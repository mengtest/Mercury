using System;
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
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        throw new ArgumentException();
        //return animator.runtimeAnimatorController.animationClips.First(clip => clip.name == clipName).length;
    }
}