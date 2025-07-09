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
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
            
            ConsentInformation.Update(requestParameters, (error) =>
                {
                    Time.timeScale = 0;
                    
                    if (error != null)
                    {
                        Debug.LogError("Consent form error: " + error.Message);
                        
                        Time.timeScale = 1;
                        return;
                    }
                    
                    ConsentForm.LoadAndShowConsentFormIfRequired((err) =>
                    {
                        if (ConsentInformation.CanRequestAds())
                        {
                            ConsentData.ConsentAllow();
                            
                            Time.timeScale = 1;
                        }
                        else
                        {
                            ConsentData.ConsentDeny();
                            Time.timeScale = 1;
                        }
                    });
                });
        }
    }
}