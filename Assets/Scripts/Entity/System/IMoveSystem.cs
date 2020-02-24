using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 运动系统接口
    /// </summary>
    public interface IMoveSystem : IEntitySystem
    {
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