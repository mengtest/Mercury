using System;
using Unity.Mathematics;

public sealed class CalculationChain : CacheCalculator<DisorderList<float>, float>
{
    private readonly Func<DisorderList<float>, float> _func;

    public CalculationChain(Func<DisorderList<float>, float> func) : base(new DisorderList<float>(4, (l, r) => math.abs(l - r) < 0.000001f))
    {
        _func = func;
        RefreshResult();
    }

    public override float Calculate() { return _func(Calculator); }

    public void Add(float chain) { Calculator.Add(chain); }

    public bool Remove(float chain) { return Calculator.Remove(chain); }
}