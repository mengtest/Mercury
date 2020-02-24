namespace Mercury
{
    public static class Misc
    {
        /// <summary>
        /// float类型的数据变化相加
        /// </summary>
        /// <returns>相加后数据</returns>
        public static float DataChangeAdd(in DataChange<float> data) { return data.raw + data.delta; }
    }
}