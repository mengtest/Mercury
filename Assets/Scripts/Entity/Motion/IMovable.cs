using UnityEngine;

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
        MotionData MotionRawData { get; }

        /// <summary>
        /// 运动计算
        /// </summary>
        IMotionCompute MoveSystem { get; }

        /// <summary>
        /// 当前速度
        /// </summary>
        Vector2 Velocity { get; }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="distance">位移矢量</param>
        void Move(Vector2 distance);
    }
}