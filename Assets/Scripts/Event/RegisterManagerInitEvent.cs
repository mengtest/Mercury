using System;

namespace Mercury
{
    /// <summary>
    /// RegisterManager初始化，在检查注册项依赖关系之前发布该事件
    /// </summary>
    public class RegisterManagerInitEvent : EventArgs
    {
        /// <summary>
        /// 初始化的RegisterManager
        /// </summary>
        public RegisterManager Registries { get; }

        public RegisterManagerInitEvent(RegisterManager registries) { Registries = registries; }
    }
}