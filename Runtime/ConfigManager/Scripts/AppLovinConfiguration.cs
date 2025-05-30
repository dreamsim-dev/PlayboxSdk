using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;

namespace Playbox.SdkConfigurations
{ 
    /// <summary>
    /// Provides configuration management for AppLovin SDK integration with Playbox, including saving and loading JSON configurations.
    /// </summary>
    public static class AppLovinConfiguration{
    
        private static string ios_key = "";
        private static string android_key = "";
        private static string advertisementSdk = "";
    
        private static bool active = false;

        private static string name = "AppLovin";

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

        public static string AdvertisementSdk
        {
            get => advertisementSdk;
            set => advertisementSdk = value;
        }

        public static string Name
        {
            get => name;
            set => name = value;
        }


        public static JObject GetJsonConfig()
        {
            JObject config = new JObject();
        
            config[nameof(ios_key)] = ios_key;
            config[nameof(android_key)] = android_key;
            config[nameof(advertisementSdk)] = AdvertisementSdk;
            config[nameof(active)] = active;
        
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
            advertisementSdk = (string)obj[nameof(advertisementSdk)];
            active = (bool)(obj[nameof(active)] ?? false);
        
        }

    }
}