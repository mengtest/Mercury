public abstract class CacheCalculator<TC, TResult>
{
    public TC Calculator { get; }
    public virtual TResult Result { get; private set; }

    public CacheCalculator(TC calculator) { Calculator = calculator; }

    public abstract TResult Calculate();

    public virtual TResult RefreshResult()
    {
        Result = Calculate();
        return Result;
    }
}