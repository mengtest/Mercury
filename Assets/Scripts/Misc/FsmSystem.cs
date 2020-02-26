using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    public class TransitionInfo
    {
        public readonly IFsmState last;
        public readonly IFsmState next;
        public readonly Func<TransitionInfo, bool> reason;

        public TransitionInfo(IFsmState last, IFsmState next, Func<TransitionInfo, bool> reason)
        {
            this.next = next;
            this.reason = reason;
            this.last = last;
        }
    }

    public class FsmSystem
    {
        private readonly Dictionary<string, IFsmState> _states;

        public IFsmState CurrentState { get; private set; }
        public IFsmState NormalState { get; set; }

        public FsmSystem() { _states = new Dictionary<string, IFsmState>(); }

        public void AddState(IFsmState state)
        {
            if (_states.ContainsKey(state.Id))
            {
                throw new ArgumentException($"已存在状态id:{state.Id}");
            }

            _states.Add(state.Id, state);
        }

        public void AddTransition(IFsmState last, IFsmState next, Func<TransitionInfo, bool> reason)
        {
            if (!_states.ContainsValue(last) || !_states.ContainsValue(next))
            {
                throw new ArgumentException($"未添加状态实例{last.Id}或{next.Id}");
            }

            var info = new TransitionInfo(last, next, reason);
            last.NextStates.Add(info);
        }

        /// <summary>
        /// 执行状态过渡
        /// </summary>
        public bool PerformTransition()
        {
            var list = CurrentState.NextStates;
            foreach (var info in list)
            {
                if (info.last != CurrentState)
                {
                    throw new InvalidOperationException("过渡映射错误");
                }

                if (info.reason == null)
                {
                    Debug.LogError($"永远也不会进入的过渡{info}");
                    continue;
                }

                if (!info.reason(info))
                {
                    continue;
                }

                CurrentState.OnLeave();
                CurrentState = info.next;
                CurrentState.OnEnter();
                return true;
            }

            return false;
        }

        public void SetCurrentState(IFsmState state)
        {
            if (CurrentState != null)
            {
                throw new InvalidOperationException();
            }

            CurrentState = state;
        }
    }
}