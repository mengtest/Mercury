using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 状态过渡信息
    /// </summary>
    public class TransitionInfo
    {
        /// <summary>
        /// 过渡前状态
        /// </summary>
        public readonly IFsmState last;

        /// <summary>
        /// 过渡后状态
        /// </summary>
        public readonly IFsmState next;

        /// <summary>
        /// 过渡原因
        /// </summary>
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
        /// <summary>
        /// 储存所有状态
        /// </summary>
        private readonly Dictionary<string, IFsmState> _states;

        /// <summary>
        /// 当前状态
        /// </summary>
        public IFsmState CurrentState { get; private set; }

        /// <summary>
        /// 默认状态
        /// </summary>
        public IFsmState NormalState { get; set; }

        public FsmSystem() { _states = new Dictionary<string, IFsmState>(); }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state">状态实例</param>
        /// <exception cref="ArgumentException">已存在状态</exception>
        public void AddState(IFsmState state)
        {
            if (_states.ContainsKey(state.Id))
            {
                throw new ArgumentException($"已存在状态id:{state.Id}");
            }

            _states.Add(state.Id, state);
        }

        /// <summary>
        /// 添加状态过渡（连接两个状态）
        /// </summary>
        /// <param name="last">上一个状态</param>
        /// <param name="next">下一个状态</param>
        /// <param name="reason">过渡原因</param>
        /// <exception cref="ArgumentException">未知状态</exception>
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
        /// 检查，执行过渡
        /// </summary>
        /// <returns>是否执行了过渡</returns>
        /// <exception cref="InvalidOperationException">该过渡与当前状态不匹配</exception>
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

        /// <summary>
        /// 设置当前状态
        /// </summary>
        /// <exception cref="InvalidOperationException">当前状态不是null</exception>
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