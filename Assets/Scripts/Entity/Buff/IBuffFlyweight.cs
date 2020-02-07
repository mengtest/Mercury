/// <summary>
/// Buff享元模式公共接口
/// </summary>
/// <typeparam name="T">实现该接口的结构体</typeparam>
public interface IBuffFlyweight
{
    /// <summary>
    /// 获取Buff享元模式原型，需注意泛型的类型是否填错
    /// </summary>
    /// <typeparam name="T">享元实例类型</typeparam>
    /// <returns>原型</returns>
    AbstractBuff<T> GetPrototype<T>() where T : struct, IBuffFlyweight;
}