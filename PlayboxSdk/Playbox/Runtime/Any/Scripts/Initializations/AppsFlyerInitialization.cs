using Playbox.SdkConfigurations;
using AppsFlyerSDK;


namespace Playbox
{
    public class AppsFlyerInitialization : PlayboxBehaviour
    {
        public override void Initialization()
        {
            base.Initialization();
            
            AppsFlyerConfiguration.LoadJsonConfig();
            
            if(!AppsFlyerConfiguration.Active)
                return;

      
#if UNITY_IOS

             AppsFlyer.initSDK(AppsFlyerConfiguration.IOSKey, AppsFlyerConfiguration.IOSAppId);
#elif UNITY_ANDROID
            
            AppsFlyer.initSDK(AppsFlyerConfiguration.AndroidKey, AppsFlyerConfiguration.AndroidAppId);
#endif 
            
            AppsFlyer.startSDK();

#if UNITY_EDITOR
            AppsFlyer.setIsDebug(true);      
#endif
            
        }
    }
}