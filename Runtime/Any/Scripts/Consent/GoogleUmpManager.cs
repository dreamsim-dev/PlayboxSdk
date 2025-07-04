using System;
using CI.Utils.Extentions;

namespace Playbox.Consent
{
#if PBX_DEVELOPMENT || UNITY_ANDROID
    
    using GoogleMobileAds.Ump.Api;
    using UnityEngine;
    
    public static class GoogleUmpManager
    {
        public static void RequestConsentInfo()
        {
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
            
            ConsentInformation.Update(requestParameters, (error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Consent form error: " + error.Message);
                        return;
                    }
                    
                    ConsentForm.LoadAndShowConsentFormIfRequired((err) =>
                    {
                        if (ConsentInformation.CanRequestAds())
                        {
                            ConsentData.ConsentAllow();
                        }
                        else
                        {
                            ConsentData.ConsentDeny();
                        }
                    });
                });
        }
    }
    
#endif
    
}