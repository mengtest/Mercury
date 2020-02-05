public struct BuffFlyweightDot
{
    public readonly DotBuff prototype;
    public readonly Entity source;
    public float Duration { get; }
    public int Intensity { get; }

    public BuffFlyweightDot(DotBuff prototype, Entity source, float duration, int intensity)
    {
        this.prototype = prototype;
        this.source = source;
        Duration = duration;
        Intensity = intensity;
    }
}