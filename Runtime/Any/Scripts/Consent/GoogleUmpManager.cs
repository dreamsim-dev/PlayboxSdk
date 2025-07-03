using System;
using CI.Utils.Extentions;

namespace Playbox.Consent
{
#if  PBX_DEVELOPMENT || UNITY_ANDROID
    
    using GoogleMobileAds.Ump.Api;
    using UnityEngine;
    
    public static class GoogleUmpManager
    {
        private static ConsentForm consentForm;
        
        public static void RequestConsentInfo()
        {
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
            
            ConsentInformation.Update(requestParameters, (error) =>
                {
                    
                    Debug.Log("Consent info updated");
    
                    if (ConsentInformation.IsConsentFormAvailable())
                    {
                        LoadConsentForm();
                    }
                    
                    if(error != null)
                        Debug.LogError("Consent update failed: " + error.Message);

                    if(Application.isEditor)
                        "PEW PEW!".PlayboxLog("CONSENT");

                });
        }
    
        static void LoadConsentForm()
        {
            ConsentForm.Load((form, error) =>
            {
                if (form != null)
                {
                    consentForm = form;

                    if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
                    {
                        ShowConsentForm();
                    }
                    else
                    {
                        ConsentData.ConsentAllow();
                        Debug.Log("Consent not required, status: " + ConsentInformation.ConsentStatus);
                    }
                }
            });
        }
    
        static void ShowConsentForm()
        {
            consentForm.Show(error =>
            {
                var status = ConsentInformation.ConsentStatus;
                
                Debug.Log("Consent form completed, status: " + status);

                if (status == ConsentStatus.Obtained)
                    ConsentData.ConsentAllow();
                else
                    ConsentData.ConsentDeny();
                
            });
        }
    }
    
#endif
    
}