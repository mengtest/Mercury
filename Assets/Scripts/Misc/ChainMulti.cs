using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    /// <summary>
    /// float类型，容器为链表，刷新缓存为将所有数据相乘，初始值为1
    /// </summary>
    public class ChainMulti : CacheData<float>
    {
        public ChainMulti() : base(new LinkedList<float>()) { }

        protected override float Refresh() { return dataContainer.Aggregate(1f, (current, variation) => current * variation); }
    }
}