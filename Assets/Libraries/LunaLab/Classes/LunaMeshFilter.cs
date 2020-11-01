using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LunaLab
{
    public class LunaMeshFilter : LunaComponent
    {
        public Mesh mesh;
        public int instanceID;

        public override void From(Object component)
        {
            MeshFilter meshFilter = (MeshFilter)component;
            mesh = meshFilter.sharedMesh;
            instanceID = meshFilter.GetInstanceID();
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject meshJson = mesh.ToJSONObject();
          

            data.SetField("instanceID", instanceID.ToString());
            data.SetField("type", "MeshFilter");
            data.SetField("mesh", meshJson.GetField("instanceID").ToString());

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
            resources.SetField(meshJson.GetField("instanceID").ToString(), meshJson);
        }
    }
}