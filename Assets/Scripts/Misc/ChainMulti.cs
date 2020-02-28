using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    /// <summary>
    /// float类型，容器为链表，刷新缓存为将所有数据相乘。
    /// 如果不包含元素，计算结果为0
    /// </summary>
    public class ChainMulti : CacheData<float>
    {
        public ChainMulti() : base(new LinkedList<float>()) { }

        protected override float Refresh()
        {
            if (dataContainer.Count == 0)
            {
                return 0;
            }

            return dataContainer.Aggregate(1f, (current, variation) => current * variation);
        }
    }
}