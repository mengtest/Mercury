using UnityEngine;

namespace Mercury
{
    public class EntityAttackable : Entity, ISystemOwner, IAttackable
    {
        public float health;

        float IAttackable.Health { get => health; set => health = value; }
        public DamageData DamageRawData { get; set; }
        public IDamageCompute DamageCompute { get; set; }
        public IDamageSystem DamageSystem { get; set; }

        public void AddSystem<T>(T system) where T : MonoBehaviour, IEntitySystem { throw new System.NotImplementedException(); }

        public T GetSystem<T>() where T : MonoBehaviour, IEntitySystem { throw new System.NotImplementedException(); }
    }
}