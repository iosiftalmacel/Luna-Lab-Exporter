using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LunaLab
{
    public class LunaGameObject: LunaObject
    {
        public int instanceID;
        public bool active;
        public string name;
        public List<LunaComponent> components = new List<LunaComponent>();
        public List<LunaGameObject> children = new List<LunaGameObject>();

        public override void From (UnityEngine.Object obj)
        {
            GameObject gameObject = (GameObject)obj;
           
            instanceID = gameObject.GetInstanceID();
            active = gameObject.activeInHierarchy;
            name = gameObject.name;

            foreach (var component in gameObject.GetComponents<Component>())
            {
                LunaComponent jsonComponent = component.ToLunaObject() as LunaComponent;
                if (jsonComponent != null) components.Add(jsonComponent);
            }

            foreach (Transform child in gameObject.transform)
                children.Add((LunaGameObject)child.gameObject.ToLunaObject());
        }

        public override void AddTo(ref JSONObject instanceIDs, ref JSONObject resources)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject jsonComponents = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject jsonChildren = new JSONObject(JSONObject.Type.ARRAY);

          

            foreach (var component in components)
                component.AddTo(ref jsonComponents, ref resources);

            foreach (var child in children)
                child.AddTo(ref jsonChildren, ref resources);

            data.SetField("instanceID", instanceID.ToString());
            data.SetField("type", "GameObject");
            data.SetField("name", name);
            data.SetField("active", active);
            data.SetField("components", jsonComponents);
            data.SetField("children", jsonChildren);

            instanceIDs.Add(instanceID.ToString());
            resources.SetField(instanceID.ToString(), data);
        }
    }
}
