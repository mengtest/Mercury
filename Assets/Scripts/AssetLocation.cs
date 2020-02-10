using System;

public struct AssetLocation : IEquatable<AssetLocation>
{
    public readonly string label;
    public readonly string type;
    public readonly string name;
    private string _fullName;

    public AssetLocation(string label, string type, string name)
    {
        this.label = label;
        this.type = type;
        this.name = name;
        _fullName = null;
    }

    //public IList<object> ToObjectList() { return new object[] {label, name}; }

    public bool Equals(AssetLocation other) { return other.label == label && other.name == name && other.type == type; }

    public override bool Equals(object obj) { return obj is AssetLocation other && Equals(other); }

    public override int GetHashCode() { return label.GetHashCode() ^ name.GetHashCode() ^ type.GetHashCode(); }

    public string GetAssetName() { return $"{type}.{name}"; }

    public override string ToString()
    {
        if (_fullName != null)
        {
            return _fullName;
        }

        _fullName = $"{label}.{type}.{name}";
        return _fullName;
    }
}