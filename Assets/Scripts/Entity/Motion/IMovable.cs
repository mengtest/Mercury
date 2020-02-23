using UnityEngine;

namespace Mercury
{
    public interface IMovable
    {
        MotionData MotionRawData { get; }

        IMotionCompute MoveSystem { get; }

        Vector2 Velocity { get; }

        void Move(Vector2 distance);
    }
}