using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    public abstract class EntityPlayer : Entity, IAttackable, IMovable
    {
        public PlayerInterface PlayerObject { get; }

        protected EntityPlayer(AssetLocation id, PlayerInterface player, DamageData damageData, MotionData motionData) : base(id)
        {
            PlayerObject = player;
            DamageRawData = damageData;
            MotionRawData = motionData;
            AddComponent(DamageRawData);
            AddComponent(MotionRawData);
        }

        public virtual void OnUpdate()
        {
            // if (_moveCapability.canMove)
            // {
            var controller = PlayerObject.CharacterController;
            var velocity = Vector2.zero;
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

            var moveSpeed = MoveSystem.MoveSpeed;
            var jumpSpeed = MoveSystem.JumpSpeed;
            var gravity = MoveSystem.Gravity;
            var gd = MoveSystem.GroundDamping;
            var ad = MoveSystem.AirDamping;
            var moveAdd = moveSpeed.raw + moveSpeed.delta;
            var jumpAdd = jumpSpeed.raw + jumpSpeed.delta;
            var gravityAdd = gravity.raw + gravity.delta;
            var gdAdd = gd.raw + gd.delta;
            var adAdd = ad.raw + ad.delta;
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

        public DamageData DamageRawData { get; }
        public abstract IDamageCompute DamageSystem { get; }

        public abstract Damage CalculateDamage(float coe, DamageType type);

        public Damage DealDamage(in Damage damage, IAttackable target)
        {
            GameManager.Instance.EventBus.Publish(this, new EntityAttackEvent(this, target, damage));
            return damage;
        }

        public abstract void UnderAttack(in Damage damage);

        public MotionData MotionRawData { get; }
        public abstract IMotionCompute MoveSystem { get; }
        public Vector2 Velocity => PlayerObject.CharacterController.velocity;

        public abstract void Move(Vector2 distance);
    }
}