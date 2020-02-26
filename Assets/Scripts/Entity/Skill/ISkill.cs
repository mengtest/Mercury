namespace Mercury
{
    public interface ISkill
    {
        AssetLocation Id { get; }

        float PerUseTime { get; }

        float PostUseTime { get; }

        bool IsDone { get; }

        bool CanUse();

        void OnPreUse();

        void OnUsing();

        void OnPostUse();
    }
}