using System;
using JetBrains.Annotations;

namespace Mercury
{
    public static class Extensions
    {
        /// <summary>
        /// 设置id
        /// </summary>
        public static T SetId<T>(this T entity, [NotNull] AssetLocation id) where T : Entity
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            NonNullCheck(entity.Id, $"不可重复设置id:{id}");
            entity.Id = id;
            return entity;
        }

        /// <summary>
        /// 设置伤害数据
        /// </summary>
        public static T SetDamageData<T>(this T entity, [NotNull] DamageData damageData) where T : Entity, IAttackable
        {
            if (damageData == null) throw new ArgumentNullException(nameof(damageData));
            NonNullCheck(entity.DamageRawData, "不可重复添加伤害数据组件");
            entity.DamageRawData = damageData;
            return entity;
        }

        /// <summary>
        /// 设置伤害计算器
        /// </summary>
        public static T SetDamageCompute<T>(this T entity, [NotNull] IDamageCompute damageCompute) where T : Entity, IAttackable
        {
            if (damageCompute == null) throw new ArgumentNullException(nameof(damageCompute));
            NonNullCheck(entity.DamageCompute, "不可重复添加伤害计算器");
            entity.DamageCompute = damageCompute;
            return entity;
        }

        /// <summary>
        /// 设置伤害系统
        /// </summary>
        public static T SetDamageSystem<T>(this T entity, [NotNull] IDamageSystem damageSystem) where T : Entity, IAttackable
        {
            if (damageSystem == null) throw new ArgumentNullException(nameof(damageSystem));
            NonNullCheck(entity.DamageSystem, "不可重复添加伤害系统");
            entity.DamageSystem = damageSystem;
            return entity;
        }

        /// <summary>
        /// 设置运动数据
        /// </summary>
        public static T SetMotionData<T>(this T entity, [NotNull] MotionData motionData) where T : Entity, IMovable
        {
            if (motionData == null) throw new ArgumentNullException(nameof(motionData));
            NonNullCheck(entity.MotionRawData, "不可重复添加运动数据组件");
            entity.MotionRawData = motionData;
            return entity;
        }

        /// <summary>
        /// 设置运动计算器
        /// </summary>
        public static T SetMotionCompute<T>(this T entity, [NotNull] IMotionCompute motionCompute) where T : Entity, IMovable
        {
            if (motionCompute == null) throw new ArgumentNullException(nameof(motionCompute));
            NonNullCheck(entity.MoveCompute, "不可重复添加运动数据计算器");
            entity.MoveCompute = motionCompute;
            return entity;
        }

        /// <summary>
        /// 设置运动系统
        /// </summary>
        public static T SetMotionSystem<T>(this T entity, [NotNull] IMoveSystem moveSystem) where T : Entity, IMovable
        {
            if (moveSystem == null) throw new ArgumentNullException(nameof(moveSystem));
            NonNullCheck(entity.MoveSystem, "不可重复添加运动系统");
            entity.MoveSystem = moveSystem;
            return entity;
        }

        /// <summary>
        /// 设置技能系统
        /// </summary>
        public static T SetSkillSystem<T>(this T entity, [NotNull] ISkillSystem skillSystem) where T : Entity, ISkillOwner
        {
            if (skillSystem == null) throw new ArgumentNullException(nameof(skillSystem));
            NonNullCheck(entity.SkillSystem, "不可重复添加技能系统");
            entity.SkillSystem = skillSystem;
            return entity;
        }

        public static void NonNullCheck(object obj, string message)
        {
            if (obj != null)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}