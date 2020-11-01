using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LunaLab
{
    public class LunaMeshRenderer : LunaComponent
    {
        public MeshFilter meshFilter;
        public Material material;
        public bool castShadows;
        public bool receiveShadows;
        public int instanceID;
        public override void From(Object component)
        {
            MeshRenderer meshRenderer = (MeshRenderer)component;
           
            meshFilter = meshRenderer.GetComponent<MeshFilter>();
            castShadows = meshRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off;
            receiveShadows = meshRenderer.receiveShadows;
            material = meshRenderer.sharedMaterial;
            instanceID = meshRenderer.GetInstanceID();
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject materialJson = material.ToJSONObject();
          
            data.AddField("instanceID", instanceID.ToString());
            data.AddField("type", "MeshRenderer");
            data.AddField("meshFilter", meshFilter.GetInstanceID().ToString());
            data.AddField("material", material.GetInstanceID().ToString());
            data.AddField("castShadows",castShadows);
            data.AddField("receiveShadows", receiveShadows);

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
            resources.SetField(material.GetInstanceID().ToString(), materialJson);
        }
    }
}