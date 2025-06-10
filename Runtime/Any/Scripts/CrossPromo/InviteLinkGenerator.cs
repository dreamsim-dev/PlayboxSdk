using System;
using AppsFlyerSDK;

namespace Playbox
{
    public class InviteLinkGenerator : PlayboxBehaviour, IAppsFlyerConversionData, IAppsFlyerUserInvite
    {
        public Action<string> OnInviteLinkGenerated;
        public Action<string> OnOpenStoreLinkGenerated;
        public Action<string> OnAppOpenAttribution;
        public Action<string> OnConversionDataSuccess;
        
        public override void Initialization()
        {
            CrossPromo.Initialize();
            
            OnInviteLinkGenerated += (link) => CrossPromo.OnInviteLinkGenerated?.Invoke(link);
            OnOpenStoreLinkGenerated += (link) => CrossPromo.OnOpenStoreLinkGenerated?.Invoke(link);
            OnConversionDataSuccess += (conversionData) => CrossPromo.OnConversionDataSucces?.Invoke(conversionData);
            OnAppOpenAttribution += (attributionData) => CrossPromo.OnAppOpenAttribution?.Invoke(attributionData);
        }

        public void onConversionDataSuccess(string conversionData)
        {
            OnConversionDataSuccess?.Invoke(conversionData);
        }

        public void onConversionDataFail(string error)
        {
            
        }

        public void onAppOpenAttribution(string attributionData)
        {
            OnAppOpenAttribution?.Invoke(attributionData);
        }

        public void onAppOpenAttributionFailure(string error)
        {
            
        }

        public void onInviteLinkGenerated(string link)
        {
            OnInviteLinkGenerated?.Invoke(link);
        }

        public void onInviteLinkGeneratedFailure(string error)
        {
        }

        public void onOpenStoreLinkGenerated(string link)
        {
            OnOpenStoreLinkGenerated?.Invoke(link);
        }
    }
}