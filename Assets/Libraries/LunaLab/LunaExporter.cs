
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
            public int width = 500;
            public int height = 600;
        }

        [System.Serializable]
        public class Export
        {
            public bool startServer = true;
            public int serverPort = 8889;
        }


        [SerializeField]
        public Window window;
        [SerializeField]
        public Export export;

        private static string templatePath;
        private static LunaExporter instance;


        private void OnEnable()
        {
            string configPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            templatePath = Path.GetDirectoryName(configPath) + "/Template";
            instance = this;
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

            Window window = instance ? instance.window : new Window();
            Export export = instance ? instance.export : new Export();

            windowJson.AddField("width", window.width);
            windowJson.AddField("height", window.height);

            gameConfig.AddField("scenes", scenesJson);
            gameConfig.AddField("window", windowJson);
            gameConfig.AddField("useShadowMap", true);

            string jsonSavePath = destination + "/game_config.json";
            File.WriteAllText(jsonSavePath, gameConfig.ToString());

            EditorUtility.RevealInFinder(destination);

            if (export.startServer)
            {
                HttpFileServer myServer = new HttpFileServer(destination, export.serverPort);
                Application.OpenURL($"http://localhost:{export.serverPort}/index.html");
            }
        }
    }

}
