using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;

namespace Playbox.SdkConfigurations
{
    /// <summary>
    /// Provides configuration management for Facebook SDK integration with Playbox, including app details and tokens.
    /// </summary>
    public static class FacebookSdkConfiguration{
    
        private static string appLabel = "";
        private static string app_id = "";
        private static string clientToken = "";
    
        private static bool active;

        private static string name = "FacebookSdk";
        
        public static bool Active
        {
            get => active;
            set => active = value;
        }

        public static string AppLabel
        {
            get => appLabel;
            set => appLabel = value;
        }

        public static string AppID
        {
            get => app_id;
            set => app_id = value;
        }

        public static string Name
        {
            get => name;
            set => name = value;
        }

        public static string ClientToken
        {
            get => clientToken;
            set => clientToken = value;
        }


        public static JObject GetJsonConfig()
        {
            JObject config = new JObject();
        
            config[nameof(AppLabel)] = AppLabel;
            config[nameof(AppID)] = AppID;
            config[nameof(ClientToken)] = ClientToken;
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
        
            AppLabel = (string)obj[nameof(AppLabel)];
            AppID = (string)obj[nameof(AppID)];
            ClientToken = (string)obj[nameof(ClientToken)];
            active = (bool)(obj[nameof(active)] ?? false);
        }

    }
}