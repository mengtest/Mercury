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

                var swordWill = entity.GetComponent<SwordWillSystem>().Init(damageSys, damageComp);
                var keyboard = entity.GetComponent<KeyboardCallbackSystem>();

                var skillSys = gameObject.GetComponent<SkillSystemImpl>();
                entity.SetSkillSystem(skillSys);
                var moonAtkE = assetManager.GetPrefab("skill", Const.RaceterMoonAtkAtked);
                var moonAtkPrefab = assetManager.GetPrefab("skill", Const.RaceterMoonAtk);
                var moonAtk = Object.Instantiate(moonAtkPrefab)
                    .GetComponent<SkillRaceterMoonAttack>()
                    .Init(Const.RaceterMoonAtk, entity, entity, KeyCode.A, gameObject, moonAtkE);
                skillSys.AddSkill(moonAtk);

                keyboard.AddCallback(KeyCode.A, 0, () => skillSys.UseSkill(Const.RaceterMoonAtk));
                keyboard.AddCallback(KeyCode.Tab, 0, () => swordWill.StartRecycleSword());

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