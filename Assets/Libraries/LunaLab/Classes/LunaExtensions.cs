using LunaLab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunaLab
{
    public static class LunaExtensions
    {
        static Dictionary<Type, Type> jsonObjectConversion = new Dictionary<Type, Type>()
        {
            { typeof(GameObject), typeof(LunaGameObject) },
            { typeof(Transform), typeof(LunaTransform) },
            { typeof(Camera), typeof(LunaCamera) },
            { typeof(MeshFilter), typeof(LunaMeshFilter) },
            { typeof(MeshRenderer), typeof(LunaMeshRenderer) },
            { typeof(Animation), typeof(LunaAnimation) },
            { typeof(Light), typeof(LunaLight) },
        };

        public static LunaObject ToLunaObject(this UnityEngine.Object obj)
        {
            Type type = obj.GetType();
            if (!jsonObjectConversion.ContainsKey(type)) return null;

            Type jsonType = jsonObjectConversion[type];
            LunaObject jsonObject = Activator.CreateInstance(jsonType) as LunaObject;
            jsonObject.From(obj);
            return jsonObject;
        }

        public static LunaScene ToLunaScene(this Scene scene)
        {
            LunaScene lunaScene = new LunaScene();
            lunaScene.From(scene);

            return lunaScene;
        }

        public static JSONObject ToJSONObject(this Vector2 vector)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            data.AddField("x", vector.x);
            data.AddField("y", vector.y);

            return data;
        }

        public static JSONObject ToJSONObject(this Vector3 vector)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            data.AddField("x", vector.x);
            data.AddField("y", vector.y);
            data.AddField("z", vector.z);

            return data;
        }

        public static JSONObject ToJSONObject(this Quaternion quaternion)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            data.AddField("x", quaternion.x);
            data.AddField("y", quaternion.y);
            data.AddField("z", quaternion.z);
            data.AddField("w", quaternion.w);

            return data;
        }

        public static JSONObject ToJSONObject(this Color color)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            data.AddField("r", color.r);
            data.AddField("g", color.g);
            data.AddField("b", color.b);
            data.AddField("a", color.a);

            return data;
        }

        public static JSONObject ToJSONObject(this Matrix4x4 matrix)
        {
            JSONObject data = new JSONObject(JSONObject.Type.ARRAY);
            data.Add(matrix.m00);
            data.Add(matrix.m01);
            data.Add(matrix.m02);
            data.Add(matrix.m03);
            data.Add(matrix.m10);
            data.Add(matrix.m11);
            data.Add(matrix.m12);
            data.Add(matrix.m13);
            data.Add(matrix.m20);
            data.Add(matrix.m21);
            data.Add(matrix.m22);
            data.Add(matrix.m23);
            data.Add(matrix.m30);
            data.Add(matrix.m31);
            data.Add(matrix.m32);
            data.Add(matrix.m33);
            return data;
        }

        public static JSONObject ToJSONObject(this Mesh mesh)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject vertices = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject triangles = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject uv = new JSONObject(JSONObject.Type.ARRAY);


            foreach (Vector3 vertex in mesh.vertices)
                vertices.Add(vertex.ToJSONObject());

            foreach (Vector2 u in mesh.uv)
                uv.Add(u.ToJSONObject());

            foreach (int id in mesh.triangles)
                triangles.Add(id);

            data.AddField("instanceID", mesh.GetInstanceID());
            data.AddField("vertices", vertices);
            data.AddField("triangles", triangles);
            data.AddField("uv", uv);

            return data;
        }

        public static JSONObject ToJSONObject(this AnimationClip clip)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject bindings = new JSONObject(JSONObject.Type.ARRAY);

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                if (binding.path != "") continue;
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                JSONObject bindingJson = new JSONObject(JSONObject.Type.OBJECT);
                JSONObject timesJson = new JSONObject(JSONObject.Type.ARRAY);
                JSONObject valuesJson = new JSONObject(JSONObject.Type.ARRAY);
                bindingJson.SetField("propertyName", binding.propertyName);

                foreach (var key in curve.keys)
                {
                    valuesJson.Add(key.value);
                    timesJson.Add(key.time);
                }

                bindingJson.AddField("times", timesJson);
                bindingJson.AddField("values", valuesJson);

                bindings.Add(bindingJson);
            }

            data.AddField("instanceID", clip.GetInstanceID());
            data.AddField("name", clip.name);
            data.AddField("wrapMode", (int)clip.wrapMode);
            data.AddField("bindings", bindings);

            return data;
        }

        public static JSONObject ToJSONObject(this Material material)
        {
            JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
            
            data.AddField("instanceID", material.GetInstanceID());
            data.AddField("color", material.color.ToJSONObject());
            data.AddField("glossiness", material.GetFloat("_Glossiness"));
            data.AddField("metallic", material.GetFloat("_Metallic"));
            data.AddField("emissionColor", material.GetColor("_EmissionColor").ToJSONObject());

            return data;
        }
    }

}