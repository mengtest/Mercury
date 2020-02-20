/// <summary>
/// 数据变化量
/// </summary>
public struct DataChange
{
    /// <summary>
    /// 原始数据
    /// </summary>
    public readonly float raw;

    /// <summary>
    /// 变化量
    /// </summary>
    public readonly float change;

    /// <summary>
    /// 和
    /// </summary>
    public float Add => raw + change;

    /// <summary>
    /// 积
    /// </summary>
    public float Multiply => raw * change;

    public DataChange(float raw, float change)
    {
        this.raw = raw;
        this.change = change;
    }
}