namespace Mercury
{
    /// <summary>
    /// 实体可以每帧调用的接口
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// 每帧调用
        /// </summary>
        void OnUpdate();
    }
}