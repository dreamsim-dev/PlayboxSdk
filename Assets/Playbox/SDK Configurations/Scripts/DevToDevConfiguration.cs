using CI.Utils.Extentions;
using DevToDev.Analytics;
using Newtonsoft.Json.Linq;

namespace Playbox.SdkConfigurations
{
    public static class DevToDevConfiguration{
    
        private static string ios_key = "";
        private static string android_key = "";
        private static DTDLogLevel logLevel = DTDLogLevel.No;
        
        private static bool active = false;

        private static string name = "DevToDev";

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

        public static DTDLogLevel LOGLevel
        {
            get => logLevel;
            set => logLevel = value;
        }


        public static JObject GetJsonConfig()
        {
            JObject config = new JObject();
        
            config[nameof(ios_key)] = ios_key;
            config[nameof(android_key)] = android_key;
            config[nameof(active)] = active;
            config[nameof(LOGLevel)] = (int)LOGLevel;
        
            return config;
        }

        public static void SaveJsonConfig()
        {
            GlobalPlayboxConfig.SaveSubconfigs(Name ,GetJsonConfig());
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
            
            if(obj.ContainsKey(nameof(LOGLevel)))
                LOGLevel = (DTDLogLevel)((int)obj[nameof(LOGLevel)]);
        }

    }
}