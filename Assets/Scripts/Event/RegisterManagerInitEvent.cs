using System;

namespace Mercury
{
    public class RegisterManagerInitEvent : EventArgs
    {
        public RegisterManager Registries { get; }

        public RegisterManagerInitEvent(RegisterManager registries) { Registries = registries; }
    }
}