using System;
using Unity.Mathematics;

public struct DamageCritChain : IEquatable<DamageCritChain>
{
    public readonly float coefficient;
    public readonly object source;

    public DamageCritChain(object source, float coefficient)
    {
        this.source = source;
        this.coefficient = coefficient;
    }

    public bool Equals(DamageCritChain other)
    {
        return other.source == source && math.abs(other.coefficient - coefficient) < 0.001f;
    }
}