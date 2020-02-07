using System;
using UnityEngine;

/// <summary>
/// 享元模式 状态Buff实例
/// </summary>
public struct BuffFlyweightState : IBuffFlyweight //<BuffFlyweightState>
{
    /// <summary>
    /// 原型
    /// </summary>
    public readonly StateBuff prototype;

    /// <summary>
    /// 来源
    /// </summary>
    public readonly Entity source;

    /// <summary>
    /// 到期时间
    /// </summary>
    public float ExpireTime { get; set; }

    /// <summary>
    /// 持续时间
    /// </summary>
    public readonly float interval;

    /// <summary>
    /// 强度
    /// </summary>
    public int intensity;

    public AbstractBuff<T> GetPrototype<T>() where T : struct, IBuffFlyweight
    {
        var cast = prototype as AbstractBuff<T>;
        if (cast == null)
        {
            throw new InvalidCastException();
        }

        return cast;
    }

    /// <param name="prototype">原型</param>
    /// <param name="source">来源</param>
    /// <param name="interval">持续时间，会自动加上当前时间，赋值给到期时间expireTime</param>
    /// <param name="intensity">强度</param>
    public BuffFlyweightState(StateBuff prototype, Entity source, float interval, int intensity)
    {
        this.prototype = prototype;
        this.source = source;
        ExpireTime = Time.time + interval;
        this.intensity = intensity;
        this.interval = interval;
    }
}