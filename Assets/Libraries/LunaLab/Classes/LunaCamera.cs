using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LunaLab
{
    public class LunaCamera : LunaComponent
    {
        public Matrix4x4 projectionMatrix;
        public Color backgroundColor;
        public float fieldOfView;
        public float nearClipPlane;
        public float farClipPlane;
        public int instanceID;


        public override void From(Object component)
        {
            Camera camera = (Camera)component;
            projectionMatrix = camera.projectionMatrix;
            backgroundColor = camera.backgroundColor;
            fieldOfView = camera.fieldOfView;
            nearClipPlane = camera.nearClipPlane;
            farClipPlane = camera.farClipPlane;
            instanceID = camera.GetInstanceID();
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
           
            data.SetField("instanceID", instanceID.ToString());
            data.SetField("type", "Camera");
            data.SetField("projectionMatrix", projectionMatrix.ToJSONObject());
            data.SetField("backgroundColor", backgroundColor.ToJSONObject());
            data.SetField("fieldOfView", fieldOfView);
            data.SetField("nearClipPlane", nearClipPlane);
            data.SetField("farClipPlane", farClipPlane);

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
        }
    }
}