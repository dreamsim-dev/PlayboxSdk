using System.Collections.Generic;
using Facebook.Unity;
using Playbox.SdkConfigurations;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace Playbox
{
    public class FacebookSdkInitialization : PlayboxBehaviour
    {
        public override void Initialization()
        {
            base.Initialization();
            
            FacebookSdkConfiguration.LoadJsonConfig();
            
            if(!FacebookSdkConfiguration.Active)
                return;
            
#if UNITY_IOS
            ATTrackingStatusBinding.RequestAuthorizationTracking();
#endif
            
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                FB.Init(FacebookSdkConfiguration.AppID,
                    FacebookSdkConfiguration.ClientToken,
                    true,
                    true,
                    true,
                    false,
                    true,
                    null,
                    "en_US",
                    null,
                    OnInitCallback);
            }
            
        }

        private void OnInitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
                ApproveInitialization();
            }
            else
            {
                Analytics.TrackEvent("Facebook", new List<KeyValuePair<string, string>>{
                    new("type","Error of Initializing"),
                    new("app identifier",Application.identifier)
                });
            }
        }

    }
}