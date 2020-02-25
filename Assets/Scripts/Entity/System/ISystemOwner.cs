namespace Mercury
{
    public interface ISystemOwner
    {
        void AddSystem<T>(T system) where T : class, IEntitySystem;

        T GetSystem<T>() where T : class, IEntitySystem;
    }
}