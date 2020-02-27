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
                var prefab = GameManager.Instance.Assets.GetPrefab("entity", id); //获取实体的预制体
                var go = Object.Instantiate(prefab); //实例化
                var cc2d = go.GetComponent<CharacterController2D>(); //获取角色控制器
                var motionData = new MotionData //运动数据
                {
                    moveSpeed = 2,
                    jumpSpeed = 1.5f,
                    groundDamping = 20f,
                    airDamping = 5f,
                    gravity = -25f
                };
                var moveCompute = new MotionComputeImpl(motionData); //运动计算器
                var moveSys = new MoveSystemImpl(moveCompute, cc2d); //运动系统
                var dmgData = new DamageData //伤害数据
                {
                    critCoe = 1.5f,
                    healthRecover = 1.265f,
                    maxHealth = 100,
                    physicsAttack = 100
                };
                var dmgCompute = new DamageComputeImpl(dmgData); //伤害计算器
                var skillSys = new SkillSystemImpl(); //技能系统
                var result = go.AddComponent<EntityPlayer>()
                    .SetMotionData(motionData)
                    .SetMotionCompute(moveCompute)
                    .SetMotionSystem(moveSys)
                    .SetDamageData(dmgData)
                    .SetDamageCompute(dmgCompute)
                    .SetSkillSystem(skillSys);
                result.SetId(id);
                var dmgSys = new DamageSystemImpl(result); //伤害系统
                result.SetDamageSystem(dmgSys);
                var keyCallback = new KeyboardCallbackSystem();
                result.AddSystem(keyCallback);
                var moonAtk = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk2);
                var moonAtkRng = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk2Rng);
                var skillMAtk = new SkillGeneralAttack(Const.RaceterMoonAtk2, //实例化皎月1,TODO:替换特效
                    result,
                    result,
                    cc2d.gameObject,
                    moonAtkRng,
                    moonAtk)
                {
                    PerUseTime = 0,
                    PostUseTime = 0.5f,
                    AttackRangeOffset = 1f,
                    DamageCoe = 75,
                    DamageType = DamageType.Physics,
                    AttackSpeed = 2
                };
                skillSys.AddSkill(skillMAtk); //添加皎月1
                keyCallback.AddCallback(KeyCode.A, 0, () => skillSys.UseSkill(Const.RaceterMoonAtk2));

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
                var result = go.AddComponent<EntityAttackable>()
                    .SetDamageData(dmgData)
                    .SetDamageCompute(dmgCompute);
                result.SetId(id);
                var dmgSys = new DamageSystemImpl(result);
                result.SetDamageSystem(dmgSys);

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