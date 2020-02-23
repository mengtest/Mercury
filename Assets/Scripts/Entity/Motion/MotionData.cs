using System;

namespace Mercury
{
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