using System;

/// <summary>
/// 注入
/// 目前仅支持Skill的资源注入
/// TODO:一套IoC DI框架？
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class InjectAttribute : Attribute
{
}