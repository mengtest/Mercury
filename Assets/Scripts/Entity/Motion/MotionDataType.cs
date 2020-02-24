namespace Mercury
{
    /// <summary>
    /// 运动数据类型
    /// </summary>
    public enum MotionDataType
    {
        /// <summary>
        /// 移动
        /// </summary>
        Move = 0,

        /// <summary>
        /// 跳跃
        /// </summary>
        Jump = 1,

        /// <summary>
        /// 地面阻力
        /// </summary>
        GroundDamping = 2,

        /// <summary>
        /// 空气阻力
        /// </summary>
        AirDamping = 3,

        /// <summary>
        /// 重力
        /// </summary>
        Gravity = 4
    }
}