using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;

namespace Playbox.SdkConfigurations
{
    public static class AppsFlyerConfiguration{
    
        private static string ios_key = "";
        private static string android_key = "";
        
        private static string android_app_Id = "";
        private static string ios_app_Id = "";
    
        private static bool active = false;

        private static string name = "AppsFlyer";

        public static string IOSKey
        {
            get => ios_key;
            set => ios_key = value;
        }

        public static string AndroidKey
        {
            get => android_key;
            set => android_key = value;
        }

        public static bool Active
        {
            get => active;
            set => active = value;
        }

        public static string Name
        {
            get => name;
            set => name = value;
        }

        public static string AndroidAppId
        {
            get => android_app_Id;
            set => android_app_Id = value;
        }

        public static string IOSAppId
        {
            get => ios_app_Id;
            set => ios_app_Id = value;
        }


        public static JObject GetJsonConfig()
        {
            JObject config = new JObject();
        
            config[nameof(ios_key)] = ios_key;
            config[nameof(android_key)] = android_key;
            config[nameof(active)] = active;
            config[nameof(ios_app_Id)] = ios_app_Id;
            config[nameof(android_app_Id)] = android_app_Id;
        
            return config;
        }

        public static void SaveJsonConfig()
        {
            GlobalPlayboxConfig.SaveSubconfigs(Name,GetJsonConfig());
        }
    
        public static void LoadJsonConfig()
        {
            JObject obj = GlobalPlayboxConfig.LoadSubconfigs(Name);

            if (obj == null)
            {
                $"{Name} config not contains in json".PlayboxWarning();
            
                return;
            }
        
            ios_key = (string)obj[nameof(ios_key)];
            android_key = (string)obj[nameof(android_key)];
            active = (bool)(obj[nameof(active)] ?? false);
            ios_app_Id = (string)obj[nameof(ios_app_Id)];
            android_app_Id = (string)obj[nameof(android_app_Id)];
        
        }

    }
}