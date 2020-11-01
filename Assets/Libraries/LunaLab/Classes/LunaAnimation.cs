using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LunaLab
{
    public class LunaAnimation : LunaComponent
    {
        public AnimationClip clip;
        public int instanceID;

        public override void From(Object component)
        {
            Animation animation = (Animation)component;
            clip = animation.clip;
            instanceID = animation.GetInstanceID();
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject clipJson = clip.ToJSONObject();

            data.SetField("instanceID", instanceID.ToString());
            data.SetField("type", "Animation");
            data.SetField("clip", clipJson.GetField("instanceID"));

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
            resources.SetField(clipJson.GetField("instanceID").ToString(), clipJson);
        }
    }
}