using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    public class EntityAttackable : Entity, IUpdatable, ISystemOwner, IAttackable
    {
        private readonly List<IUpdatable> _updatableSystem;
        private readonly LinkedList<IEntitySystem> _systems;

        public EntityAttackable(AssetLocation id) : base(id)
        {
            _updatableSystem = new List<IUpdatable>();
            _systems = new LinkedList<IEntitySystem>();
        }

        public float Health { get; set; }
        public DamageData DamageRawData { get; private set; }
        public IDamageCompute DamageCompute { get; private set; }
        public IDamageSystem DamageSystem { get; private set; }

        public void OnUpdate()
        {
            foreach (var sys in _updatableSystem)
            {
                sys.OnUpdate();
            }
        }

        public EntityAttackable SetDamageData(DamageData damageData)
        {
            NonNullCheck(DamageRawData, "不可重复添加伤害数据组件");
            DamageRawData = damageData;
            AddComponent(DamageRawData);
            return this;
        }

        public EntityAttackable SetDamageCompute(IDamageCompute damageCompute)
        {
            NonNullCheck(DamageCompute, "不可重复添加伤害计算器");
            DamageCompute = damageCompute;
            return this;
        }

        public EntityAttackable SetDamageSystem(IDamageSystem damageSystem)
        {
            NonNullCheck(DamageSystem, "不可重复添加伤害系统");
            DamageSystem = damageSystem;
            AddSystem(DamageSystem);
            return this;
        }

        public void AddSystem<T>(T system) where T : class, IEntitySystem
        {
            _systems.AddLast(system);
            if (system is IUpdatable s)
            {
                _updatableSystem.Add(s);
            }
        }

        public T GetSystem<T>() where T : class, IEntitySystem
        {
            var type = typeof(T);
            return (from sys in _systems where sys.GetType() == type select sys as T).FirstOrDefault();
        }
    }
}