using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 带返回值的事件
    /// </summary>
    /// <typeparam name="TType">事件类型</typeparam>
    /// <typeparam name="TReturn">返回值类型</typeparam>
    public delegate TReturn ReturnableEvent<in TType, out TReturn>(object sender, TType e) where TType : EventArgs;

    /// <summary>
    /// 杂项，存放工具函数
    /// </summary>
    public static class Misc
    {
        /// <summary>
        /// float类型的数据变化相加
        /// </summary>
        /// <returns>相加后数据</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AddDataChange(in DataChange<float> data) { return data.raw + data.delta; }

        /// <summary>
        /// float类型的数据变化相乘
        /// </summary>
        /// <returns>相乘后数据</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MultiDataChange(in DataChange<float> data) { return data.raw * data.delta; }

        /// <summary>
        /// 每帧数据变化
        /// </summary>
        /// <param name="raw">原始数据</param>
        /// <param name="delta">变化量</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DataChangePreTick(float raw, float delta) { return raw + delta * Time.deltaTime; }


        /// <summary>
        /// 暴击数值转换为暴击率
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RecoverCritPr(float value)
        {
            if (math.abs(value) < 0.000001f)
            {
                return 0;
            }

            var a = value * 0.003f;
            return a / (a + 1);
        }
    }
}