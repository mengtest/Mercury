using Prime31;

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

        public static readonly EntityEntry Raceter = new EntityEntry(new AssetLocation("mercury", "raceter"),
            id =>
            {
                var prefab = GameManager.Instance.Assets.GetPrefab("entity", id);
                var cc2d = UnityEngine.Object.Instantiate(prefab).GetComponent<CharacterController2D>();
                var motionData = new MotionData
                {
                    moveSpeed = 2,
                    jumpSpeed = 1.5f,
                    groundDamping = 20f,
                    airDamping = 5f,
                    gravity = -25f
                };
                var moveCompute = new MotionComputeImpl(motionData);
                var unityCc2d = new UnityObject<CharacterController2D>(cc2d);
                var moveSys = new MoveSystemImpl(moveCompute, unityCc2d);
                var dmgData = new DamageData
                {
                    critCoe = 1.5f,
                    healthRecover = 1.265f,
                    maxHealth = 100,
                    physicsAttack = 100
                };
                var dmgCompute = new DamageComputeImpl(dmgData);

                var result = new EntityPlayer(id)
                    .SetMotionData(motionData)
                    .SetMotionCompute(moveCompute)
                    .SetMotionSystem(moveSys)
                    .SetDamageData(dmgData)
                    .SetDamageCompute(dmgCompute);
                result.AddComponent(unityCc2d);
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
        }
    }
}