using Prime31;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    public class MoveImpl : IMoveSystem
    {
        private readonly IMotionCompute _moveComp;
        private readonly CharacterController2D _cc2d;

        public MoveImpl(IMotionCompute moveComp, UnityObject<CharacterController2D> unityObject)
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

            var moveAdd = Misc.DataChangeAdd(moveCompute.MoveSpeed);
            var jumpAdd = Misc.DataChangeAdd(moveCompute.JumpSpeed);
            var gravityAdd = Misc.DataChangeAdd(moveCompute.Gravity);
            var gdAdd = Misc.DataChangeAdd(moveCompute.GroundDamping);
            var adAdd = Misc.DataChangeAdd(moveCompute.AirDamping);
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