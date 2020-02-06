public interface IBuffable
{
    void OnUpdateBuffs();

    void AddBuff(BuffFlyweightDot dot);

    bool RemoveDotBuff<T>() where T : DotBuff;

    bool ContainsDotBuff<T>() where T : DotBuff;

    bool TryGetDotBuff<T>(out BuffFlyweightDot dot);
}