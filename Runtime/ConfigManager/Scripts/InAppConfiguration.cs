using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;

namespace Playbox.SdkConfigurations
{
    public class InAppConfiguration
    {
        private static bool useCustomInApp = false;
    
        private static bool active = false;

        private static string name = "IAP";
        

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

        public static bool UseCustomInApp
        {
            get => useCustomInApp;
            set => useCustomInApp = value;
        }


        public static JObject GetJsonConfig()
        {
            JObject config = new JObject();
        
            config[nameof(UseCustomInApp)] = UseCustomInApp;
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
        
            UseCustomInApp = (bool)(obj[nameof(UseCustomInApp)] ?? false);
            active = (bool)(obj[nameof(active)] ?? false);
            
        }
    }
}