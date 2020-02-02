using UnityEngine;

public interface IMoveable
{
    /// <summary>
    /// 移动速度，单位（格/秒）
    /// </summary>
    float MoveSpeed { get; set; }

    /// <summary>
    /// 跳跃高度，单位（格/秒）
    /// </summary>
    float JumpSpeed { get; set; }

    /// <summary>
    /// 地面阻力
    /// </summary>
    float GroundDamping { get; set; }

    /// <summary>
    /// 空气阻力
    /// </summary>
    float AirDamping { get; set; }

    /// <summary>
    /// 重力
    /// </summary>
    float Gravity { get; set; }

    Vector2 Velocity { get; set; }

    void Move(Vector2 velocity);
}