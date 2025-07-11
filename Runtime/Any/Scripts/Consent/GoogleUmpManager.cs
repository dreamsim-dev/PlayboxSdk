using System;
using CI.Utils.Extentions;

namespace Playbox.Consent
{
    using GoogleMobileAds.Ump.Api;
    using UnityEngine;
    
    public static class GoogleUmpManager
    {
        public static void RequestConsentInfo()
        {
            "Consent Info Prod".PlayboxInfo();
            
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
            
            ConsentInformation.Update(requestParameters, (error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Consent form error: " + error.Message);
                    
                        ConsentData.ConsentDeny();
                        return;
                    }
                    ConsentForm.LoadAndShowConsentFormIfRequired((err) =>
                    {
                        if (error != null)
                        {
                            ConsentData.ConsentDeny();
                            return;
                        }

                        if (ConsentInformation.CanRequestAds())
                        {
                            ConsentData.ConsentAllow();
                            return;
                        }
                        else
                        {
                            ConsentData.ConsentDeny();
                            return;
                        }
                    });
                });
        }
        
        public static void RequestConsentInfoDebug(ConsentDebugSettings consentDebugSettings)
        {
            "Consent Info Debug".PlayboxInfo();
            
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings =  consentDebugSettings
            };
            
            ConsentInformation.Update(requestParameters, (error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Consent form error: " + error.Message);
                    
                    ConsentData.ConsentDeny();
                    return;
                }
                    
                ConsentForm.LoadAndShowConsentFormIfRequired((err) =>
                {
                    if (error != null)
                    {
                        ConsentData.ConsentDeny();
                        return;
                    }
                    if (ConsentInformation.CanRequestAds())
                    {
                        ConsentData.ConsentAllow();
                        return;
                    }
                    else
                    {
                        ConsentData.ConsentDeny();
                        return;
                    }
                });
            });
        }
    }
}