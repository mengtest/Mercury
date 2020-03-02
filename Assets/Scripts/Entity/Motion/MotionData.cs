using System;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 运动原始数据
    /// </summary>
    [Serializable]
    public class MotionData : MonoBehaviour, IEntityData
    {
        /// <summary>
        /// 移动速度，单位（格/秒）
        /// </summary>
        [Header("移动速度，单位（格/秒）")] public float moveSpeed = 1f;

        /// <summary>
        /// 跳跃高度，单位（格/秒）
        /// </summary>
        [Header("跳跃高度，单位（格/秒）")] public float jumpSpeed = 1.5f;

        /// <summary>
        /// 地面阻力
        /// </summary>
        [Header("地面阻力")] public float groundDamping = 20;

        /// <summary>
        /// 空气阻力
        /// </summary>
        [Header("空气阻力")] public float airDamping = 5;

        /// <summary>
        /// 重力
        /// </summary>
        [Header("重力")] public float gravity = -25;
    }
}