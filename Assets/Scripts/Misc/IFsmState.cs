using System.Collections.Generic;

namespace Mercury
{
    public interface IFsmState
    {
        string Id { get; }

        FsmSystem System { get; }

        IList<TransitionInfo> NextStates { get; }

        /// <summary>
        /// 进入本状态时调用
        /// </summary>
        void OnEnter();

        /// <summary>
        /// 每帧调用
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 离开本状态时调用
        /// </summary>
        void OnLeave();
    }

    public class FsmStateImpl : IFsmState
    {
        public string Id { get; }
        public FsmSystem System { get; }
        public IList<TransitionInfo> NextStates { get; }

        public FsmStateImpl(string id, FsmSystem system, IList<TransitionInfo> nextStates)
        {
            Id = id;
            System = system;
            NextStates = nextStates;
            system.AddState(this);
        }

        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnLeave() { }
    }
}