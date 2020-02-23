namespace Mercury
{
    public struct DataChange<T> where T : struct
    {
        public readonly T raw;
        public readonly T delta;

        public DataChange(T raw, T delta)
        {
            this.raw = raw;
            this.delta = delta;
        }
    }
}