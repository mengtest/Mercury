using System.Linq;
using UnityEngine;

public static class SkillUtility
{
    /// <summary>
    /// 获取动画长度
    /// </summary>
    /// <param name="animator">动画机</param>
    /// <param name="clipName">动画名</param>
    /// <returns></returns>
    public static float GetClipLength(Animator animator, string clipName)
    {
        return animator.runtimeAnimatorController.animationClips.First(clip => clip.name == clipName).length;
    }
}