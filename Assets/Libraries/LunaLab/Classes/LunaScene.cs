using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunaLab
{
 
    public class LunaScene
    {
        public string name;
        public List<LunaGameObject> hierarchy = new List<LunaGameObject>();

        public void From (Scene scene)
        {
            foreach (GameObject gameObject in scene.GetRootGameObjects())
                hierarchy.Add((LunaGameObject)gameObject.ToLunaObject());
           
            name = scene.name;
        }

        public JSONObject ToJsonObject()
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject jsonHierarchy = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject jsonResources = new JSONObject(JSONObject.Type.OBJECT);

            foreach (var child in hierarchy)
                child.AddTo(ref jsonHierarchy, ref jsonResources);

            data.SetField("type", "Scene");
            data.SetField("name", name);
            data.SetField("hierarchy", jsonHierarchy);
            data.SetField("resources", jsonResources);

            return data;
        }
    }
}
