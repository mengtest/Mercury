using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 键盘按键回调
    /// </summary>
    public class KeyboardCallbackSystem : IEntitySystem, IUpdatable
    {
        private List<(KeyCode, Action)>[] _callbacks;

        public KeyboardCallbackSystem()
        {
            _callbacks = new List<(KeyCode, Action)>[3];
            _callbacks[0] = new List<(KeyCode, Action)>();
            _callbacks[1] = new List<(KeyCode, Action)>();
            _callbacks[2] = new List<(KeyCode, Action)>();
        }

        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="keyCode">按键</param>
        /// <param name="keyState">按键状态，0表示按下，1表示抬起/释放，2表示一直按着</param>
        /// <param name="action"></param>
        public void AddCallback(KeyCode keyCode, int keyState, Action action) { _callbacks[keyState].Add((keyCode, action)); }

        public void OnUpdate()
        {
            foreach (var (keyCode, action) in _callbacks[0])
            {
                if (Input.GetKeyDown(keyCode))
                {
                    action();
                }
            }

            foreach (var (keyCode, action) in _callbacks[1])
            {
                if (Input.GetKeyUp(keyCode))
                {
                    action();
                }
            }

            foreach (var (keyCode, action) in _callbacks[2])
            {
                if (Input.GetKey(keyCode))
                {
                    action();
                }
            }
        }
    }
}