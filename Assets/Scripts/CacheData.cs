using System.Collections.Generic;

namespace Mercury
{
    public abstract class CacheData<T>
    {
        protected readonly ICollection<T> _dataContainer;

        protected CacheData(ICollection<T> dataContainer) { _dataContainer = dataContainer; }

        public T Result { get; private set; }

        public abstract T Refresh();

        public void DataChange() { Result = Refresh(); }

        public void AddData(T data)
        {
            _dataContainer.Add(data);
            DataChange();
        }

        public bool RemoveData(T data)
        {
            var result = _dataContainer.Remove(data);
            DataChange();
            return result;
        }
    }
}