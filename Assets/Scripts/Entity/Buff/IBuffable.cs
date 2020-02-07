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

    bool RemoveDotBuff(string buffName);

    bool RemoveStateBuff(string buffName);

    BuffFlyweightDot GetDotBuff(string buffName);

    BuffFlyweightState GetStateBuff(string buffName);

    bool ContainsDotBuff(string buffName);

    bool ContainsStateBuff(string buffName);

    bool TryGetDotBuff(string buffName, out BuffFlyweightDot dot);

    bool TryGetStateBuff(string buffName, out BuffFlyweightState state);
}