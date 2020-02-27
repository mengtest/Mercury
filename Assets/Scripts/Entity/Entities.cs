using Prime31;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 存放所有需注册的Entity
    /// </summary>
    public static class Entities
    {
        /// <summary>
        /// Entity注册表
        /// </summary>
        public static readonly IRegistry<EntityEntry> Registry = new RegistryImpl<EntityEntry>("entity");

        public static readonly EntityEntry Raceter = new EntityEntry(Const.Raceter,
            id =>
            {
                var prefab = GameManager.Instance.Assets.GetPrefab("entity", id);
                var cc2d = Object.Instantiate(prefab).GetComponent<CharacterController2D>();
                var entityRef = cc2d.gameObject.AddComponent<EntityReference>();
                var motionData = new MotionData
                {
                    moveSpeed = 2,
                    jumpSpeed = 1.5f,
                    groundDamping = 20f,
                    airDamping = 5f,
                    gravity = -25f
                };
                var moveCompute = new MotionComputeImpl(motionData);
                var unityCc2d = new UnityObject<CharacterController2D>("cc2d", cc2d);
                var moveSys = new MoveSystemImpl(moveCompute, unityCc2d);
                var dmgData = new DamageData
                {
                    critCoe = 1.5f,
                    healthRecover = 1.265f,
                    maxHealth = 100,
                    physicsAttack = 100
                };
                var dmgCompute = new DamageComputeImpl(dmgData);
                var skillSys = new SkillSystemImpl();

                var result = new EntityPlayer(id)
                    .SetMotionData(motionData)
                    .SetMotionCompute(moveCompute)
                    .SetMotionSystem(moveSys)
                    .SetDamageData(dmgData)
                    .SetDamageCompute(dmgCompute)
                    .SetSkillSystem(skillSys);
                result.AddComponent(unityCc2d);
                entityRef.Entity = result;
                result.AddComponent(entityRef);
                var dmgSys = new DamageSystemImpl(result);
                result.SetDamageSystem(dmgSys);
                var moonAtk = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk2);
                var moonAtkRng = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk2Rng);
                var skillMAtk = new SkillGeneralAttack(Const.RaceterMoonAtk2,
                    result,
                    cc2d.gameObject,
                    moonAtkRng,
                    moonAtk)
                {
                    PerUseTime = 0,
                    PostUseTime = 0.5f,
                    PosOffset = 1f
                };
                skillSys.AddSkill(skillMAtk);
                result.DamageSystem.OnDealDamage += (sender, e) =>
                {
                    Debug.Log($"攻击!{e.source},{e.target},{e.damage.value}");
                    e.Result = new Damage(e.source, 233, 666, e.damage.type);
                };

                return result;
            });

        public static readonly EntityEntry Scarecrow = new EntityEntry(Const.Scarecrow,
            id =>
            {
                var dmgData = new DamageData
                {
                    maxHealth = 10000,
                    healthRecover = 10,
                };
                var dmgCompute = new DamageComputeImpl(dmgData);
                var prefab = GameManager.Instance.Assets.GetPrefab("entity", id);
                var go = Object.Instantiate(prefab);
                var entityRef = go.AddComponent<EntityReference>();
                var c = new UnityObject<GameObject>("gameObject", go);
                var result = new EntityAttackable(id, EntityType.Enemy)
                    .SetDamageData(dmgData)
                    .SetDamageCompute(dmgCompute);
                entityRef.Entity = result;
                var dmgSys = new DamageSystemImpl(result);
                result.SetDamageSystem(dmgSys);
                result.AddComponent(c);
                result.AddComponent(entityRef);
                result.DamageSystem.OnUnderAttack += (sender, e) =>
                {
                    Debug.Log($"啊啊啊啊啊啊!{e.source},{e.target},{e.damage}");
                };

                return result;
            });

        /// <summary>
        /// 初始化注册表
        /// </summary>
        /// <param name="manager">注册表管理</param>
        public static void Init(RegisterManager manager)
        {
            manager.AddRegistry(Registry);
            Registry.Register(Raceter);
            Registry.Register(Scarecrow);
        }
    }
}