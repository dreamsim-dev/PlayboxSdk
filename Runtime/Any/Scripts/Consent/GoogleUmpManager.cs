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
                    
                    switch (ConsentInformation.ConsentStatus)
                    {
                        case ConsentStatus.NotRequired:
                        case ConsentStatus.Obtained:
                            ConsentData.ConsentAllow(); 
                            break;

                        case ConsentStatus.Required:
                            if (ConsentInformation.IsConsentFormAvailable())
                                LoadConsentForm(); 
                            else
                                ConsentData.ConsentDeny();
                            break;

                        case ConsentStatus.Unknown:
                        default:
                            ConsentData.ConsentAllow(); 
                            break;
                    }
                    
                    if(error != null)
                        Debug.LogError("Consent update failed: " + error.Message);

                    if(Application.isEditor)
                        "PEW PEW!".PlayboxLog("CONSENT");

                });
        }
    
        private static void LoadConsentForm()
        {
            ConsentForm.Load((form, error) =>
            {
                if (form != null)
                {
                    if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
                    {
                        form.Show(_ =>
                        {
                            var status = ConsentInformation.ConsentStatus;
                
                            Debug.Log("Consent form completed, status: " + status);

                            if (status == ConsentStatus.Obtained)
                                ConsentData.ConsentAllow();
                            if (status == ConsentStatus.NotRequired)
                            {
                                ConsentData.ConsentDeny();
                            }
                        });
                    }
                }
            });
        }
    }
    
#endif
    
}