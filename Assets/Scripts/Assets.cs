namespace Mercury
{
    /// <summary>
    /// 存放所有需注册的资源
    /// </summary>
    public static class Assets
    {
        /// <summary>
        /// 资源注册表
        /// </summary>
        public static readonly IRegistry<AssetEntry> Registry = new RegistryImpl<AssetEntry>("asset");

        public static readonly AssetEntry RaceterPrefab = new AssetEntry("entity", new AssetLocation("mercury", "raceter"));

        public static void Init(RegisterManager manager)
        {
            manager.AddRegistry(Registry);
            Registry.Register(RaceterPrefab);
        }
    }
}