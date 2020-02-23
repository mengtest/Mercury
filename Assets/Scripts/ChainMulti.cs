using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    public class ChainMulti : CacheData<float>
    {
        public ChainMulti() : base(new LinkedList<float>()) { }

        public override float Refresh() { return _dataContainer.Aggregate(1f, (current, variation) => current * variation); }
    }
}