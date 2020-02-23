using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    public class ChainAdd : CacheData<float>
    {
        public ChainAdd() : base(new LinkedList<float>()) { }

        public override float Refresh() { return _dataContainer.Sum(); }
    }
}