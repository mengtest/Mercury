/// <summary>
/// Buff享元模式公共接口
/// </summary>
/// <typeparam name="T">实现该接口的结构体</typeparam>
public interface IBuffFlyweight<T> where T : struct, IBuffFlyweight<T>
{
    AbstractBuff<T> Prototype { get; }
}