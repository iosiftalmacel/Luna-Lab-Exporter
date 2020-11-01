using System;

namespace LunaLab
{
    public abstract class LunaObject
    {
        public abstract void From(UnityEngine.Object obj);
        public abstract void AddTo(ref JSONObject instanceIDs, ref JSONObject resources);
    }
}
