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
                var result = new EntityRaceter(id,
                    cc2d,
                    new DamageData(),
                    new MotionData
                    {
                        moveSpeed = 2,
                        jumpSpeed = 1.5f,
                        groundDamping = 20f,
                        airDamping = 5f,
                        gravity = -25f
                    });
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