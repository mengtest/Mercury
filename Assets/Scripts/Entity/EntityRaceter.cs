using Prime31;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 南歌子实现
    /// </summary>
    public class EntityRaceter : EntityPlayer
    {
        /// <summary>
        /// Unity组件
        /// </summary>
        private readonly CharacterController2D _raceter;
        
        public EntityRaceter(AssetLocation id, CharacterController2D player, DamageData damageData, MotionData motionData) : base(id, damageData, motionData)
        {
            _raceter = player;
            MoveSystem = new MotionComputeImpl(this);
            DamageSystem = null; //TODO:伤害系统实现
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            // if (_moveCapability.canMove)
            // {
            var controller = _raceter;
            var velocity = Velocity;
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

            var moveAdd = Misc.DataChangeAdd(MoveSystem.MoveSpeed);
            var jumpAdd = Misc.DataChangeAdd(MoveSystem.JumpSpeed);
            var gravityAdd = Misc.DataChangeAdd(MoveSystem.Gravity);
            var gdAdd = Misc.DataChangeAdd(MoveSystem.GroundDamping);
            var adAdd = Misc.DataChangeAdd(MoveSystem.AirDamping);
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

        public override IDamageCompute DamageSystem { get; }
        public override Damage CalculateDamage(float coe, DamageType type) { throw new System.NotImplementedException(); }

        public override void UnderAttack(in Damage damage) { throw new System.NotImplementedException(); }

        public override IMotionCompute MoveSystem { get; }
        public override Vector2 Velocity => _raceter.velocity;
        public override void Move(Vector2 distance) { _raceter.move(distance); }
    }
}