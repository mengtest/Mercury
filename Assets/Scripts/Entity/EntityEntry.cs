public class EntityEntry : IRegistryEntry
{
    public AssetLocation RegisterName { get; }

    public EntityEntry(AssetLocation registerName) { RegisterName = registerName; }
}