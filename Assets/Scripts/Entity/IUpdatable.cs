namespace Mercury
{
    /// <summary>
    /// 每帧调用接口
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// 每帧调用
        /// </summary>
        void OnUpdate();
    }
}