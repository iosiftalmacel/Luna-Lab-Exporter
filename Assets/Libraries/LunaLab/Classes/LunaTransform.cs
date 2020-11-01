using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LunaLab
{
    public class LunaTransform : LunaComponent
    {
        public Vector3 localScale;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public int instanceID;

        public override void From(Object component)
        {
            Transform transform = (Transform)component;
            localScale = transform.localScale;
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
            instanceID = transform.GetInstanceID();
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            data.AddField("instanceID", instanceID.ToString());
            data.AddField("type", "Transform");
            data.AddField("localScale", localScale.ToJSONObject());
            data.AddField("localPosition", localPosition.ToJSONObject());
            data.AddField("localRotation", localRotation.ToJSONObject());

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
        }
    }
}