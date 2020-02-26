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

        public static readonly AssetEntry RaceterPrefab = new AssetEntry("entity", Const.Raceter);
        public static readonly AssetEntry RaceterMoonAtk2 = new AssetEntry("skill", Const.RaceterMoonAtk2); //TODO:封装成skill注册表
        public static readonly AssetEntry RaceterMoonAtk2Rng = new AssetEntry("skill", Const.RaceterMoonAtk2Rng);

        public static readonly AssetEntry ScarecrowPrefab = new AssetEntry("entity", Const.Scarecrow);

        public static void Init(RegisterManager manager)
        {
            manager.AddRegistry(Registry);
            Registry.Register(RaceterPrefab);
            Registry.Register(ScarecrowPrefab);
            Registry.Register(RaceterMoonAtk2);
            Registry.Register(RaceterMoonAtk2Rng);
        }
    }
}