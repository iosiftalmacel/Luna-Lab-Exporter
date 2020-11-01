
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
namespace LunaLab
{

    public class LunaExporter : ScriptableObject
    {
        [System.Serializable]
        public class Window
        {
            public int width;
            public int height;
        }

        [SerializeField]
        public Window window;
        private static string templatePath;


        private void OnEnable()
        {
            string configPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            templatePath = Path.GetDirectoryName(configPath) + "/Template";
        }

        [MenuItem("LunaLab/Export To HTML5")]
        static void ExportToHtml5()
        {
            if (templatePath == null) templatePath = Application.dataPath + "/Libraries/LunaLab/Template";

            Scene activeScene = SceneManager.GetActiveScene();
            var destination = EditorUtility.SaveFilePanel("Export to Html", "", activeScene.name, "");
            destination = Path.ChangeExtension(destination, null);

            Directory.CreateDirectory(destination);
            FileUtil.CopyFileOrDirectory(templatePath + "/index.html", destination + "/index.html");
            FileUtil.CopyFileOrDirectory(templatePath + "/js", destination + "/js");

            JSONObject gameConfig = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject scenesJson = new JSONObject(JSONObject.Type.ARRAY);
            JSONObject windowJson = new JSONObject(JSONObject.Type.OBJECT);

            LunaScene lunaScene = activeScene.ToLunaScene();
            scenesJson.Add(lunaScene.ToJsonObject());

            windowJson.AddField("width", 500);
            windowJson.AddField("height", 600);

            gameConfig.AddField("scenes", scenesJson);
            gameConfig.AddField("window", windowJson);
            gameConfig.AddField("useShadowMap", true);

            string jsonSavePath = destination + "/game_config.json";
            File.WriteAllText(jsonSavePath, gameConfig.ToString());

            EditorUtility.RevealInFinder(destination);


            HttpFileServer myServer = new HttpFileServer(destination);
            Application.OpenURL("http://localhost:8889/index.html");
        }
    }

}
