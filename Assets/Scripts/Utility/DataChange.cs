public struct DataChange
{
    public readonly float raw;
    public readonly float change;

    public float Add => raw + change;

    public float Multiply => raw * change;

    public DataChange(float raw, float change)
    {
        this.raw = raw;
        this.change = change;
    }
}