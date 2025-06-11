using System;
using CI.Utils.Extentions;

namespace Playbox.Consent
{
    using GoogleMobileAds.Ump.Api;
    using UnityEngine;
    
    public class GoogleUmpManager : PlayboxBehaviour
    {
        private ConsentForm consentForm;

        private void Awake()
        {
            MainInitialization.PostInitialization += st;
        }

        void st()
        {
            RequestConsentInfo();
        }
    
        void RequestConsentInfo()
        {
            ConsentRequestParameters requestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
    
            // Обновляем информацию о согласии
            ConsentInformation.Update(requestParameters, (error) =>
                {
                    Debug.Log("Consent info updated");
    
                    if (ConsentInformation.IsConsentFormAvailable())
                    {
                        LoadConsentForm();
                    }
                    
                    Debug.LogError("Consent update failed: " + error.Message);
                    
                });
        }
    
        void LoadConsentForm()
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
    
        void ShowConsentForm()
        {
            consentForm.Show(error => Debug.Log("Consent form closed.") );
        }
    }
}