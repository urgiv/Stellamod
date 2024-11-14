﻿namespace Stellamod.Common.LoadingSystems
{
    public class HookGroup : IOrderedLoadable
    {
        public virtual float Priority => 1f;

        public virtual void Load() { }

        public virtual void Unload() { }
    }
}
