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
                /*开始基础、系统、数据的组装*/
                var prefab = GameManager.Instance.Assets.GetPrefab("entity", id); //获取实体的预制体
                var go = Object.Instantiate(prefab); //实例化
                var cc2d = go.GetComponent<CharacterController2D>(); //获取角色控制器
                var motionData = new MotionData //运动数据
                {
                    moveSpeed = 3.2f,
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
                /*结束基础系统、数据的组装*/
                /*开始额外系统的组装*/
                var swordWillSys = new SwordWillSystem(dmgSys, dmgCompute); //剑意
                result.AddSystem(swordWillSys);
                /*结束额外系统的组装*/
                /*开始技能的组装*/
                //所有技能所需资源的获取
                var moonAtkAtked = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtkAtked);
                var moonAtk1Rng = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk1);
                var moonAtk2Rng = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk2);
                var moonAtk3Rng = GameManager.Instance.Assets.GetPrefab("skill", Const.RaceterMoonAtk3);

                var skMoonAtk1 = new SkillGeneralAttack(Const.RaceterMoonAtk1, //皎月1
                    result,
                    result,
                    cc2d.gameObject,
                    Object.Instantiate(moonAtk1Rng),
                    moonAtkAtked)
                {
                    PerUseTime = 0,
                    PostUseTime = 2f,
                    DamageCoe = 75,
                    DamageType = DamageType.Physics,
                    AttackTime = 0.4f,
                    AttackSpeed = 1f
                };
                skillSys.AddSkill(skMoonAtk1);

                var moonAtk2 = new SkillMultiAttack(Const.RaceterMoonAtk2, //皎月2
                    result,
                    result,
                    cc2d.gameObject,
                    Object.Instantiate(moonAtk2Rng),
                    moonAtkAtked)
                {
                    PerUseTime = 0,
                    PostUseTime = 0.1f,
                    AttackRangeOffset = 0f,
                    DamageCoe = 65,
                    DamageType = DamageType.Physics,
                    AttackSpeed = 1,
                    AttackCount = 2,
                    AttackInterval = 0.15f
                };
                skillSys.AddSkill(moonAtk2);

                var skMoonAtk3 = new SkillGeneralAttack(Const.RaceterMoonAtk3, //皎月1
                    result,
                    result,
                    cc2d.gameObject,
                    Object.Instantiate(moonAtk3Rng),
                    moonAtkAtked)
                {
                    PerUseTime = 0,
                    PostUseTime = 2f,
                    AttackRangeOffset = new Vector2(-0.25f, 0.5f),
                    DamageCoe = 155,
                    DamageType = DamageType.Physics,
                    AttackTime = 0.4f,
                    AttackSpeed = 1f
                };
                skillSys.AddSkill(skMoonAtk3);

                keyCallback.AddCallback(KeyCode.A, 0, () =>
                {
                    var res = skillSys.UseSkill(Const.RaceterMoonAtk1);
                    Debug.Log($"use result {res}");
                });
                keyCallback.AddCallback(KeyCode.S, 0, () => skillSys.UseSkill(Const.RaceterMoonAtk2));
                keyCallback.AddCallback(KeyCode.D, 0, () =>
                {
                    var res = skillSys.UseSkill(Const.RaceterMoonAtk3);
                    Debug.Log($"use result {res}");
                });
                keyCallback.AddCallback(KeyCode.Tab, 0, () => swordWillSys.StartRecycleSword()); //设置收刀的按键
                /*结束技能的组装*/
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
                dmgSys.OnUnderAttack += (o, attack) =>
                {
                    var dmg = attack.damage;
                    WorldData.ShowDamageText(dmg.FinalDamage.ToString(), dmg.type.ToString(), result.transform);
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