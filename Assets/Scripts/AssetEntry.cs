namespace Mercury
{
    /// <summary>
    /// 资源注册项
    /// </summary>
    public class AssetEntry : RegistryEntryImpl<AssetEntry>
    {
        private readonly string _type;

        public AssetEntry(string type, AssetLocation registerName)
        {
            SetRegisterName(registerName);
            _type = type;
        }

        public override string ToString() { return $"{RegisterName.Label}.{_type}.{RegisterName.Name}"; }
    }
}