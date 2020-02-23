using System;

namespace Mercury
{
    /// <summary>
    /// 资产路径，可用于表示游戏内注册项，资源路径等
    /// </summary>
    [Serializable]
    public class AssetLocation
    {
        private readonly string _toString;

        /// <summary>
        /// 资产标签
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// 资产名
        /// </summary>
        public string Name { get; }

        public AssetLocation(string label, string name)
        {
            Label = label;
            Name = name;
            _toString = $"{Label}:{Name}";
        }

        public override string ToString() { return _toString; }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override bool Equals(object obj) { return obj is AssetLocation location && Label == location.Label && Name == location.Name; }
    }
}