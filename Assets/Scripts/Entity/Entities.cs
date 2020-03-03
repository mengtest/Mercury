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
                var assetManager = GameManager.Instance.Assets;
                var prefab = assetManager.GetPrefab("entity", id);
                var gameObject = Object.Instantiate(prefab);
                var entity = gameObject.GetComponent<EntityPlayer>().SetId(id);
                var cc2d = gameObject.GetComponent<CharacterController2D>();

                var motionData = gameObject.GetComponent<MotionData>();
                var motionComp = new MotionComputeImpl(motionData);
                var moveSys = gameObject.GetComponent<MoveSystemImpl>().Init(motionComp, cc2d);
                entity.SetMotionCompute(motionComp).SetMotionData(motionData).SetMotionSystem(moveSys);

                var damageData = gameObject.GetComponent<DamageData>();
                var damageComp = new DamageComputeImpl(damageData);
                entity.SetDamageCompute(damageComp);
                var damageSys = gameObject.GetComponent<DamageSystemImpl>().Init(entity);
                entity.SetDamageData(damageData).SetDamageSystem(damageSys);

                entity.GetComponent<SwordWillSystem>().Init(damageSys, damageComp);
                var keyboard = entity.GetComponent<KeyboardCallbackSystem>();

                var skillSys = gameObject.GetComponent<SkillSystemImpl>();
                entity.SetSkillSystem(skillSys);
                var moonAtkE = assetManager.GetPrefab("skill", Const.RaceterMoonAtkAtked);
                var moonAtk1 = assetManager.GetPrefab("skill", Const.RaceterMoonAtk1);
                var moonAtk2 = assetManager.GetPrefab("skill", Const.RaceterMoonAtk2);
                var moonAtk3 = assetManager.GetPrefab("skill", Const.RaceterMoonAtk3);
                var moonAtk1Ins = Object.Instantiate(moonAtk1)
                    .GetComponent<SkillGeneralAttack>()
                    .Init(Const.RaceterMoonAtk1, entity, entity, gameObject, moonAtkE);
                skillSys.AddSkill(moonAtk1Ins);
                var moonAtk2Ins = Object.Instantiate(moonAtk2)
                    .GetComponent<SkillGeneralAttack>()
                    .Init(Const.RaceterMoonAtk2, entity, entity, gameObject, moonAtkE);
                skillSys.AddSkill(moonAtk2Ins);
                var moonAtk3Ins = Object.Instantiate(moonAtk3)
                    .GetComponent<SkillMultiAttack>()
                    .Init(Const.RaceterMoonAtk3, entity, entity, gameObject, moonAtkE);
                skillSys.AddSkill(moonAtk3Ins);

                keyboard.AddCallback(KeyCode.A, 0, () => skillSys.UseSkill(Const.RaceterMoonAtk1));
                keyboard.AddCallback(KeyCode.S, 0, () => skillSys.UseSkill(Const.RaceterMoonAtk2));
                keyboard.AddCallback(KeyCode.D, 0, () => skillSys.UseSkill(Const.RaceterMoonAtk3));

                return entity;
            });

        public static readonly EntityEntry Scarecrow = new EntityEntry(Const.Scarecrow,
            id =>
            {
                var prefab = GameManager.Instance.Assets.GetPrefab("entity", id);
                var gameObject = Object.Instantiate(prefab);
                var entity = gameObject.GetComponent<EntityAttackable>().SetId(id);

                var damageData = gameObject.GetComponent<DamageData>();
                var damageComp = new DamageComputeImpl(damageData);
                entity.SetDamageCompute(damageComp);
                var damageSys = gameObject.GetComponent<DamageSystemImpl>().Init(entity);
                entity.SetDamageData(damageData).SetDamageSystem(damageSys);
                damageSys.OnUnderAttack += (o, attack) =>
                {
                    var dmg = attack.damage;
                    WorldData.ShowDamageText(dmg.FinalDamage.ToString(), dmg.type.ToString(), gameObject.transform);
                };

                return entity;
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