namespace Mercury
{
    public interface ISkill
    {
        /// <summary>
        /// 技能临时id
        /// </summary>
        AssetLocation Id { get; }

        /// <summary>
        /// 前摇
        /// </summary>
        float PerUseTime { get; }

        /// <summary>
        /// 后摇
        /// </summary>
        float PostUseTime { get; }

        /// <summary>
        /// 技能是否施放完毕
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// 是否可以使用本技能
        /// </summary>
        bool CanUse();

        /// <summary>
        /// 每帧调用OnUsing前，第一次使用该技能
        /// </summary>
        void OnPreUse();

        /// <summary>
        /// 每帧调用，正在使用该技能
        /// </summary>
        void OnUsing();

        /// <summary>
        /// 结束，离开技能时调用
        /// </summary>
        void OnPostUse();
    }
}