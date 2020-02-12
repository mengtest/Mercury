public interface IBuffable
{
    void OnUpdateBuffs();

    void AddBuff(BuffStack buff);

    bool RemoveBuff(AssetLocation location);

    bool HasBuff(AssetLocation location);

    BuffStack GetBuff(AssetLocation location);

    bool TryGetBuff(AssetLocation location, out BuffStack buff);
}