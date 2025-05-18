using System.IO;
using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Playbox.SdkConfigurations
{
    public static class GlobalPlayboxConfig 
    {
        private static JObject jsonConfig = new();
        private static string configFile = "playbox_sdk_config.json";
        private static string configBackupFile = "playbox_sdk_config_backup.json";
    
        public static void Load()
        {
            var textAsset = Resources.Load<TextAsset>($"Playbox/PlayboxConfig/{configFile}");

            if (textAsset != null)
            {
                jsonConfig = JObject.Parse(textAsset.text);
            }
        }

        public static void Save()
        {
            var path = Path.Combine(Application.streamingAssetsPath,"Playbox", "PlayboxConfig");
        
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        
            File.WriteAllText(Path.Combine(path,configFile), jsonConfig.ToString());
        
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    
        private static void SaveBackup()
        {
            var path = Path.Combine(Application.streamingAssetsPath,"Playbox", "PlayboxConfig");
        
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        
            File.WriteAllText(Path.Combine(path,configBackupFile), jsonConfig.ToString());

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public static void SaveSubconfigs(string name, JObject config)
        {
            jsonConfig[name] = config;
        }

        public static JObject LoadSubconfigs(string name)
        {
            if (jsonConfig[name] == null)
                return null;
            
            return jsonConfig[name].ToObject<JObject>();
        }

        public static void Clear()
        {
            SaveBackup();
        
            jsonConfig = new JObject();
        }
    }
}
