﻿using CI.Utils.Extentions;

namespace Playbox.Consent
{
    using GoogleMobileAds.Ump.Api;
    using UnityEngine;
    
    public static class GoogleUmpManager
    {
        private static ConsentForm consentForm;
        
        public static void SubscribeToPreInit()
        {
            MainInitialization.PreInitialization += RequestConsentInfo;
        }
        
        static void RequestConsentInfo()
        {
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
            
            ConsentInformation.Update(requestParameters, (error) =>
                {
                    if (error == null)
                        return;
                    
                    Debug.Log("Consent info updated");
    
                    if (ConsentInformation.IsConsentFormAvailable())
                    {
                        LoadConsentForm();
                    }
                    
                    Debug.LogError("Consent update failed: " + error.Message);

                    if(Application.isEditor)
                        "PEW PEW!".PlayboxLog("CONSENT");

                });
        }
    
        static void LoadConsentForm()
        {
            ConsentForm.Load((form, error) =>
            {
                
                if (error != null)
                {
                    Debug.LogError("Consent form load failed: " + error.Message);
                    return;
                }
    
                if (form == null)
                {
                    Debug.LogError("Consent form is null!");
                    return;
                }
    
                consentForm = form;
    
                if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
                {
                    ShowConsentForm();
                    
                    AppConsent.hasUserConsent = true;
                }
                else
                {
                    AppConsent.hasUserConsent = false;
                    Debug.Log("Consent not required, status: " + ConsentInformation.ConsentStatus);
                }
            });
        }
    
        static void ShowConsentForm()
        {
            consentForm.Show(error => Debug.Log("Consent form closed.") );
        }
    }
}