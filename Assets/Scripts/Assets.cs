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
        public static readonly AssetEntry RaceterMoonAtkAtked = new AssetEntry("skill", Const.RaceterMoonAtkAtked);
        public static readonly AssetEntry RaceterMoonAtk1Eff = new AssetEntry("skill", Const.RaceterMoonAtk1);
        public static readonly AssetEntry RaceterMoonAtk2Eff = new AssetEntry("skill", Const.RaceterMoonAtk2);
        public static readonly AssetEntry RaceterMoonAtk3Eff = new AssetEntry("skill", Const.RaceterMoonAtk3);
        public static readonly AssetEntry RaceterMoonAtk = new AssetEntry("skill", Const.RaceterMoonAtk);

        public static readonly AssetEntry ScarecrowPrefab = new AssetEntry("entity", Const.Scarecrow);

        public static readonly AssetEntry TextDamagePhysics = new AssetEntry("effect", Const.TextDamagePhy);
        public static readonly AssetEntry TextDamageMagic = new AssetEntry("effect", Const.TextDamageMag);
        public static readonly AssetEntry TextDamageTrue = new AssetEntry("effect", Const.TextDamageTru);

        public static void Init(RegisterManager manager)
        {
            manager.AddRegistry(Registry);
            Registry.Register(RaceterPrefab);
            Registry.Register(ScarecrowPrefab);

            Registry.Register(RaceterMoonAtkAtked);
            Registry.Register(RaceterMoonAtk1Eff);
            Registry.Register(RaceterMoonAtk2Eff);
            Registry.Register(RaceterMoonAtk3Eff);
            Registry.Register(RaceterMoonAtk);

            Registry.Register(TextDamagePhysics);
            Registry.Register(TextDamageMagic);
            Registry.Register(TextDamageTrue);
        }
    }
}