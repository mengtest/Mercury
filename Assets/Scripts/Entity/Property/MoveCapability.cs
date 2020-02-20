using System;
using UnityEngine;

[Serializable]
public class MoveCapability : IEntityProperty
{
    /// <summary>
    /// 当前跳跃的计数，重置时赋值为maxJumpCount。
    /// </summary>
    public int jumpCount;

    /// <summary>
    /// 最大的多段跳跃次数
    /// </summary>
    public int maxJumpCount = 2;

    /// <summary>
    /// 跳跃之后的冷却时间。
    /// </summary>
    public float maxJumpCD = 0.2f;

    /// <summary>
    /// 是否可以进行移动
    /// </summary>
    public bool canMove = true;

    /// <summary>
    /// 受到的重力，每秒下落速度。
    /// </summary>
    public float gravity = -25f;

    /// <summary>
    /// 移动速度。
    /// </summary>
    public float runSpeed = 1f;

    /// <summary>
    /// 当角色处在于地面受到的阻力。
    /// </summary>
    public float groundDamping = 20f;

    /// <summary>
    /// 当角色处在于空中时受到的阻力。
    /// </summary>
    public float inAirDamping = 5f;

    /// <summary>
    /// 跳跃高度。
    /// </summary>
    public float jumpHeight = 1.5f;

    /// <summary>
    /// 当前位移量，
    /// </summary>
    public Vector2 velocity;

    /// <summary>
    /// 下一次跳跃时间
    /// </summary>
    private float _nextJumpTime;

    /// <summary>
    /// 直接调用该函数，返回值如果为True则可以跳跃
    /// </summary>
    public bool IsJumpReady => jumpCount > 0 && _nextJumpTime <= Time.time;

    /// <summary>
    /// 使用该函数可以重置跳跃次数
    /// </summary>
    public void RecoverJumpCount() { jumpCount = maxJumpCount; }

    /// <summary>
    /// 测试能不能跳跃，如果返回值为True则可以跳跃，同时使跳跃进入冷却时间并且计算一次跳跃次数。
    /// </summary>
    public bool TryJump()
    {
        if (!IsJumpReady || !canMove)
        {
            return false;
        }

        jumpCount -= 1;
        _nextJumpTime = Time.time + maxJumpCD;
        return true;
    }
}