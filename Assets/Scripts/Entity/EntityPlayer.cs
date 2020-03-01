using System;
using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 玩家
    /// </summary>
    public sealed class EntityPlayer : Entity, IUpdatable, ISystemOwner, IAttackable, IMovable, ISkillOwner
    {
        private Dictionary<Type, IEntitySystem> _system;
        private List<IUpdatable> _updatableSystem;

        protected override void Awake()
        {
            base.Awake();
            _system = new Dictionary<Type, IEntitySystem>();
            _updatableSystem = new List<IUpdatable>();
        }

        public void OnUpdate()
        {
            foreach (var system in _updatableSystem)
            {
                system.OnUpdate();
            }
        }

        public float Health { get; set; }
        public DamageData DamageRawData { get; private set; }
        public IDamageCompute DamageCompute { get; private set; }
        public IDamageSystem DamageSystem { get; private set; }
        public MotionData MotionRawData { get; private set; }
        public IMotionCompute MoveCompute { get; private set; }
        public IMoveSystem MoveSystem { get; private set; }

        public void AddSystem<T>(T system) where T : class, IEntitySystem
        {
            _system.Add(typeof(T), system);
            if (system is IUpdatable s)
            {
                _updatableSystem.Add(s);
            }
        }

        public T GetSystem<T>() where T : class, IEntitySystem
        {
            if (!_system.TryGetValue(typeof(T), out var system))
            {
                return null;
            }

            if (system is T ins)
            {
                return ins;
            }

            return null;
        }

        public EntityPlayer SetDamageData(DamageData damageData) //TODO:扩展方法
        {
            NonNullCheck(DamageRawData, "不可重复添加伤害数据组件");
            DamageRawData = damageData;
            AddComponent(DamageRawData);
            return this;
        }

        public EntityPlayer SetDamageCompute(IDamageCompute damageCompute)
        {
            NonNullCheck(DamageCompute, "不可重复添加伤害计算器");
            DamageCompute = damageCompute;
            return this;
        }

        public EntityPlayer SetDamageSystem(IDamageSystem damageSystem)
        {
            NonNullCheck(DamageSystem, "不可重复添加伤害系统");
            DamageSystem = damageSystem;
            AddSystem(DamageSystem);
            return this;
        }

        public EntityPlayer SetMotionData(MotionData motionData)
        {
            NonNullCheck(MotionRawData, "不可重复添加运动数据组件");
            MotionRawData = motionData;
            AddComponent(MotionRawData);
            return this;
        }

        public EntityPlayer SetMotionCompute(IMotionCompute motionCompute)
        {
            NonNullCheck(MoveCompute, "不可重复添加运动数据计算器");
            MoveCompute = motionCompute;
            return this;
        }

        public EntityPlayer SetMotionSystem(IMoveSystem moveSystem)
        {
            NonNullCheck(MoveSystem, "不可重复添加运动系统");
            MoveSystem = moveSystem;
            AddSystem(MoveSystem);
            return this;
        }

        public EntityPlayer SetSkillSystem(ISkillSystem skillSystem)
        {
            NonNullCheck(SkillSystem, "不可重复添加技能系统");
            SkillSystem = skillSystem;
            AddSystem(SkillSystem);
            return this;
        }

        public ISkillSystem SkillSystem { get; private set; }
    }
}