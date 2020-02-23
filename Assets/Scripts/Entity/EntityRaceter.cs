using UnityEngine;

namespace Mercury
{
    public class EntityRaceter : EntityPlayer
    {
        public EntityRaceter(AssetLocation id, PlayerInterface player, DamageData damageData, MotionData motionData) : base(id, player, damageData, motionData)
        {
            MoveSystem = new MotionComputeImpl(this);
            DamageSystem = null; //TODO:伤害系统实现
        }

        public override IDamageCompute DamageSystem { get; }
        public override Damage CalculateDamage(float coe, DamageType type) { throw new System.NotImplementedException(); }

        public override void UnderAttack(in Damage damage) { throw new System.NotImplementedException(); }

        public override IMotionCompute MoveSystem { get; }
        public override void Move(Vector2 distance) { PlayerObject.CharacterController.move(distance); }
    }
}