using System;

namespace Mercury
{
    /// <summary>
    /// 运动原始数据
    /// </summary>
    [Serializable]
    public class MotionData : IEntityComponent
    {
        /// <summary>
        /// 移动速度，单位（格/秒）
        /// </summary>
        public float moveSpeed;

        /// <summary>
        /// 跳跃高度，单位（格/秒）
        /// </summary>
        public float jumpSpeed;

        /// <summary>
        /// 地面阻力
        /// </summary>
        public float groundDamping;

        /// <summary>
        /// 空气阻力
        /// </summary>
        public float airDamping;

        /// <summary>
        /// 重力
        /// </summary>
        public float gravity;
    }
}