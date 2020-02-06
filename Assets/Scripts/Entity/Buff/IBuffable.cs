/// <summary>
/// 可使用Buff
/// </summary>
public interface IBuffable
{
    /// <summary>
    /// buff更新
    /// </summary>
    void OnUpdateBuffs();

    /// <summary>
    /// 添加Dot buff
    /// </summary>
    void AddBuff(BuffFlyweightDot dot);

    /// <summary>
    /// 添加状态Buff
    /// </summary>
    void AddBuff(BuffFlyweightState state);

    bool RemoveDotBuff<T>() where T : DotBuff;

    bool RemoveStateBuff<T>() where T : StateBuff;

    bool ContainsDotBuff<T>() where T : DotBuff;

    bool ContainsStateBuff<T>() where T : StateBuff;

    bool TryGetDotBuff<T>(out BuffFlyweightDot dot);

    bool TryGetStateBuff<T>(out BuffFlyweightState state);
}