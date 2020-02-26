using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Mercury
{
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
        public static float AddData(in DataChange<float> data) { return data.raw + data.delta; }

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