using System.Collections.Generic;

namespace Mercury
{
    public class NormalState : FsmStateImpl
    {
        public NormalState(string id, FsmSystem system) : base(id, system, new List<TransitionInfo>(1)) { }
    }
}