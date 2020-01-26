using System.Linq;
using UnityEngine;

/// <summary>
/// 抽象技能
/// </summary>
public abstract class AbstractSkill : IFSMState
{
    protected readonly ISkillable skillHolder;
    protected readonly float cd;
    protected float lastUse = float.MinValue;

    public FSMSystem System => skillHolder.Skills;

    protected AbstractSkill(ISkillable holder, float cd)
    {
        skillHolder = holder;
        this.cd = cd;
    }

    /// <summary>
    /// 检查该技能是否准备好
    /// </summary>
    public abstract bool CanEnter();

    /// <summary>
    /// 每帧调用
    /// </summary>
    public abstract void OnAct();

    /// <summary>
    /// 释放技能时调用
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// 技能结束时调用
    /// </summary>
    public abstract void OnLeave();

    /// <summary>
    /// 是否冷却完毕
    /// </summary>
    protected bool IsCoolDown() { return Time.time - lastUse >= cd; }

    /// <summary>
    /// 刷新冷却时间
    /// </summary>
    protected void RefreshCoolDown() { lastUse = Time.time; }

    /// <summary>
    /// 获取动画长度
    /// </summary>
    /// <param name="animator">动画机</param>
    /// <param name="clipName">动画名</param>
    /// <returns></returns>
    public static float GetClipLength(Animator animator, string clipName)
    {
        return animator.runtimeAnimatorController.animationClips.First((clip) => clip.name == clipName).length;
    }

    /// <summary>
    /// 进入后摇
    /// </summary>
    /// <param name="time">后摇长度</param>
    protected void EnterStiffness(float time)
    {
        skillHolder.UseSkill<StiffnessState>(out var state);
        state.Duration = time;
    }

    /// <summary>
    /// 当前释放中技能
    /// </summary>
    protected IFSMState CurrentSkill() { return System.CurrentState; }
}