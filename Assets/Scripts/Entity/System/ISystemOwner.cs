namespace Mercury
{
    public interface ISystemOwner : IUpdatable
    {
        void AddSystem<T>(T system) where T : class, IEntitySystem;

        T GetSystem<T>() where T : class, IEntitySystem;
    }
}