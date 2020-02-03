using System;
using System.Collections.Generic;

public struct AssetLocation : IEquatable<AssetLocation>
{
    public readonly string label;
    public readonly string name;

    public AssetLocation(string label, string name)
    {
        this.label = label;
        this.name = name;
    }

    public List<object> ToObjectList() { return new List<object> {label, name}; }

    public bool Equals(AssetLocation other) { return other.label == label && other.name == name; }

    public override bool Equals(object obj) { return obj is AssetLocation other && Equals(other); }

    public override int GetHashCode() { return label.GetHashCode() ^ name.GetHashCode(); }

    public override string ToString() { return $"{label}:{name}"; }
}