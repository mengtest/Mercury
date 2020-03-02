using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 玩家
    /// </summary>
    public sealed class EntityPlayer : Entity, ISystemOwner, IAttackable, IMovable, ISkillOwner
    {
        public float health;

        public float Health { get => health; set => health = value; }
        public DamageData DamageRawData { get; set; }
        public IDamageCompute DamageCompute { get; set; }
        public IDamageSystem DamageSystem { get; set; }
        public MotionData MotionRawData { get; set; }
        public IMotionCompute MoveCompute { get; set; }
        public IMoveSystem MoveSystem { get; set; }
        public ISkillSystem SkillSystem { get; set; }

        public void AddSystem<T>(T system) where T : MonoBehaviour, IEntitySystem { gameObject.AddComponent<T>(); }

        public T GetSystem<T>() where T : MonoBehaviour, IEntitySystem { return gameObject.GetComponent<T>(); }
    }
}