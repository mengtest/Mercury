using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    /// <summary>
    /// float类型，容器为链表，刷新缓存为将所有数据相加
    /// </summary>
    public class ChainAdd : CacheData<float>
    {
        public ChainAdd() : base(new LinkedList<float>()) { }

        protected override float Refresh() { return dataContainer.Sum(); }
    }
}