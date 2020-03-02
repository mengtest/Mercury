namespace Mercury
{
    /// <summary>
    /// 运动接口
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        MotionData MotionRawData { get; set; }

        /// <summary>
        /// 运动计算
        /// </summary>
        IMotionCompute MoveCompute { get; set; }

        /// <summary>
        /// 运动系统
        /// </summary>
        IMoveSystem MoveSystem { get; set; }
    }
}