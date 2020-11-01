using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace LunaLab
{
    public class LunaLight : LunaComponent
    {

        public Color color;
        public string category;
        public float intensity;
        public float range;
        public float angle;
        public bool castShadows;
        public int instanceID;

        public override void From(Object component)
        {
            Light light = (Light)component;
            color = light.color;
            angle = light.spotAngle;
            intensity = light.intensity;
            range = light.range;
            castShadows = light.shadows != LightShadows.None;
            category = light.type.ToString();
            instanceID = light.GetInstanceID();
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);

            data.SetField("instanceID", instanceID.ToString()); ;
            data.SetField("type", "Light");
            data.SetField("category", category);
            data.SetField("range", range);
            data.SetField("angle", angle);
            data.SetField("intensity", intensity);
            data.SetField("castShadows", castShadows);
            data.SetField("color", color.ToJSONObject());

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
        }
    }
}