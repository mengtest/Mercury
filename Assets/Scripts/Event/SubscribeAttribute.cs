using System;

namespace Mercury
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SubscribeAttribute : Attribute
    {
    }
}