using System;

public struct AssetLocation : IEquatable<AssetLocation>
{
    public readonly string label;
    public readonly string type;
    public readonly string name;

    public AssetLocation(string label, string type, string name)
    {
        this.label = label;
        this.type = type;
        this.name = name;
    }

    //public IList<object> ToObjectList() { return new object[] {label, name}; }

    public bool Equals(AssetLocation other) { return other.label == label && other.name == name && other.type == type; }

    public override bool Equals(object obj) { return obj is AssetLocation other && Equals(other); }

    public override int GetHashCode() { return label.GetHashCode() ^ name.GetHashCode() ^ type.GetHashCode(); }

    public override string ToString() { return $"{label}:{type}:{name}"; }
}