using System;
using Unity.Mathematics;

public enum CritProbabilityIncomeType
{
    Value,
    Percentage
}

public struct DamageCritProbabilityChain : IEquatable<DamageCritProbabilityChain>
{
    public readonly float probability;
    public readonly CritProbabilityIncomeType incomeType;
    public readonly object source;

    public DamageCritProbabilityChain(float probability, CritProbabilityIncomeType incomeType, object source)
    {
        this.probability = probability;
        this.incomeType = incomeType;
        this.source = source;
    }

    public bool Equals(DamageCritProbabilityChain other)
    {
        return math.abs(other.probability - probability) < 0.001f &&
               other.incomeType == incomeType &&
               other.source == source;
    }
}