namespace Mercury
{
    /// <summary>
    /// 数据变化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct DataChange<T> where T : struct
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        public readonly T raw;

        /// <summary>
        /// 变化量
        /// </summary>
        public readonly T delta;

        public DataChange(T raw, T delta)
        {
            this.raw = raw;
            this.delta = delta;
        }
    }
}