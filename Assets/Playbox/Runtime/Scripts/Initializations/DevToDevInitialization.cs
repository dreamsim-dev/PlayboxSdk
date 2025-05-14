using DevToDev.Analytics;
using Playbox.SdkConfigurations;


namespace Playbox
{
    public class DevToDevInitialization : PlayboxBehaviour
    {

        public override void Initialization()
        {
            GlobalPlayboxConfig.Load();
            
            DevToDevConfiguration.LoadJsonConfig();
            
            if(!DevToDevConfiguration.Active)
                return;

#if UNITY_ANDROID
            DTDAnalytics.Initialize(DevToDevConfiguration.AndroidKey);
#endif
#if UNITY_IOS
            DTDAnalytics.Initialize(DevToDevConfiguration.IOSKey);
#endif
            DTDAnalytics.SetLogLevel(DevToDevConfiguration.LOGLevel);
            
            DTDAnalytics.StartActivity();

        }

        public override void Close()
        {
            base.Close();
            
            DTDAnalytics.StopActivity();
        }
    }
}
