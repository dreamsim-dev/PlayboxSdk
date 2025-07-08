#if PBX_DEVELOPMENT || UNITY_IOS
using System;
using System.Collections;
using AppsFlyerSDK;
using Unity.Advertisement.IosSupport;
using UnityEngine;

namespace Playbox.Consent
{
    public class IOSConsent
    {
        public static void ShowConsentUI(MonoBehaviour mono)
        {

            mono.StartCoroutine(IosATTStatus(10, status =>
            {
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
                {
                    ConsentData.ConsentAllow();
      
                }

                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED)
                {
                    ConsentData.ConsentDeny();
                }
                
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED)
                {
                    ConsentData.ConsentDeny();
                }
                
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    ConsentData.ConsentDeny();
                }
            }));
        }

        private static IEnumerator IosATTStatus(float timeout, Action<ATTrackingStatusBinding.AuthorizationTrackingStatus> action)
        {
            yield return new WaitForSeconds(0.4f);
            
#if UNITY_EDITOR
            
            action?.Invoke(ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED);
            
            yield break;
#endif
            var attStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (attStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }

            AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(20);
            
            float elapsed = 0f;
            
            while (elapsed < timeout)
            {
                var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
                if (status != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    break;
                }

                yield return new WaitForSeconds(0.5f);
                elapsed += 0.5f;
            }

            var finalStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            
            action?.Invoke(finalStatus);
        }
    }
}

#endif