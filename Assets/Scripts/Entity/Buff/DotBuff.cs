/// <summary>
/// 公共抽象Dot buff
/// </summary>
public abstract class DotBuff : AbstractBuff<BuffFlyweightDot>
{
    /// <summary>
    /// 触发时调用
    /// </summary>
    /// <param name="holder">buff持有者</param>
    /// <param name="buff">享元</param>
    public abstract void OnTrigger(IBuffable holder, in BuffFlyweightDot buff);
}