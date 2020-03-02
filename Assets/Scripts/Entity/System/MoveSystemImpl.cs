using Prime31;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 运动系统接口，应该允许每帧调用
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

    /// <summary>
    /// TODO:多段跳
    /// TODO:按键移动到keyboard callback
    /// </summary>
    public class MoveSystemImpl : MonoBehaviour, IMoveSystem
    {
        private IMotionCompute _moveComp;
        private CharacterController2D _cc2d;

        public MoveSystemImpl Init(IMotionCompute moveComp, CharacterController2D cc2d)
        {
            _moveComp = moveComp;
            _cc2d = cc2d;
            return this;
        }

        private void Update()
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
                var scale = _cc2d.transform.localScale;
                _cc2d.transform.localScale = new Vector3(math.abs(scale.x), scale.y, scale.z);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                normalizedHorizontalSpeed = -1;
                var scale = _cc2d.transform.localScale;
                _cc2d.transform.localScale = new Vector3(-math.abs(scale.x), scale.y, scale.z);
            }
            else
            {
                normalizedHorizontalSpeed = 0;
            }

            var moveAdd = Misc.AddDataChange(moveCompute.MoveSpeed);
            var jumpAdd = Misc.AddDataChange(moveCompute.JumpSpeed);
            var gravityAdd = Misc.AddDataChange(moveCompute.Gravity);
            var gdAdd = Misc.AddDataChange(moveCompute.GroundDamping);
            var adAdd = Misc.AddDataChange(moveCompute.AirDamping);
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

            Move(velocity * Time.deltaTime);
            // }
        }

        public Vector2 Velocity => _cc2d.velocity;

        public void Move(Vector2 distance) { _cc2d.move(distance); }
    }
}