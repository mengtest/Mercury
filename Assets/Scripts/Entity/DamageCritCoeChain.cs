using System;
using Unity.Mathematics;

public struct DamageCritCoeChain : IEquatable<DamageCritCoeChain>
{
    public readonly float coefficient;
    public readonly object source;

    public DamageCritCoeChain(object source, float coefficient)
    {
        this.source = source;
        this.coefficient = coefficient;
    }

    public bool Equals(DamageCritCoeChain other)
    {
        return other.source == source && math.abs(other.coefficient - coefficient) < 0.001f;
    }
}