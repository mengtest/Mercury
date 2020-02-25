using Prime31;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 运动系统接口，应该允许每帧调用
    /// </summary>
    public interface IMoveSystem : IEntitySystem, IUpdatable
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

    public class MoveSystemImpl : IMoveSystem
    {
        private readonly IMotionCompute _moveComp;
        private readonly CharacterController2D _cc2d;

        public MoveSystemImpl(IMotionCompute moveComp, UnityObject<CharacterController2D> unityObject)
        {
            _moveComp = moveComp;
            _cc2d = unityObject.Value;
        }

        public void OnUpdate()
        {
            // if (_moveCapability.canMove)
            // {
            var controller = _cc2d;
            var moveCompute = _moveComp;
            var velocity = _cc2d.velocity;
            if (controller.isGrounded)
            {
                velocity.y = 0;
                // _moveCapability.RecoverJumpCount();
            }

            float normalizedHorizontalSpeed;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                normalizedHorizontalSpeed = 1;
                // Rotate(Face.Right);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                normalizedHorizontalSpeed = -1;
                // Rotate(Face.Left);
            }
            else
            {
                normalizedHorizontalSpeed = 0;
            }

            var moveAdd = Misc.AddData(moveCompute.MoveSpeed);
            var jumpAdd = Misc.AddData(moveCompute.JumpSpeed);
            var gravityAdd = Misc.AddData(moveCompute.Gravity);
            var gdAdd = Misc.AddData(moveCompute.GroundDamping);
            var adAdd = Misc.AddData(moveCompute.AirDamping);
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // if (_moveCapability.TryJump())
                // {
                velocity.y = math.sqrt(2f * jumpAdd * -gravityAdd);
                // }
            }

            var smoothedMovementFactor = controller.isGrounded ? gdAdd : adAdd;
            velocity.x = math.lerp(velocity.x, normalizedHorizontalSpeed * moveAdd, Time.deltaTime * smoothedMovementFactor);
            velocity.y += gravityAdd * Time.deltaTime;
            if (controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
            {
                velocity.y *= 11f;
                controller.ignoreOneWayPlatformsThisFrame = true;
            }

            _cc2d.move(velocity * Time.deltaTime);
            // }
        }

        public Vector2 Velocity => _cc2d.velocity;

        public void Move(Vector2 distance) { _cc2d.move(distance); }
    }
}