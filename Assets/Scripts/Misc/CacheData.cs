using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 数据缓存
    /// </summary>
    public abstract class CacheData<T>
    {
        /// <summary>
        /// 数据容器
        /// </summary>
        protected readonly ICollection<T> dataContainer;

        protected CacheData(ICollection<T> dataContainer) { this.dataContainer = dataContainer; }

        /// <summary>
        /// 缓存
        /// </summary>
        public T Cache { get; private set; }

        /// <summary>
        /// 刷新缓存的实际方法
        /// </summary>
        /// <returns></returns>
        protected abstract T Refresh();

        /// <summary>
        /// 数据变化时调用
        /// </summary>
        private void DataChange() { Cache = Refresh(); }

        /// <summary>
        /// 添加数据
        /// </summary>
        public void AddData(T data)
        {
            dataContainer.Add(data);
            DataChange();
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        public bool RemoveData(T data)
        {
            var result = dataContainer.Remove(data);
            DataChange();
            return result;
        }
    }
}