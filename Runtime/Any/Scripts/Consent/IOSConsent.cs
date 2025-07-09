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
        public static void ShowATTUI(MonoBehaviour mono, Action onComplete)
        {

            mono.StartCoroutine(IosATTStatus(10, status =>
            {
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
                {
                    onComplete?.Invoke();
                }

                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED)
                {
              
                }
                
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED)
                {
                    
                }
                
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    
                }
                
                onComplete?.Invoke();
            }));
        }

        private static IEnumerator IosATTStatus(float timeout, Action<ATTrackingStatusBinding.AuthorizationTrackingStatus> action)
        {
            Time.timeScale = 0;
            yield return new WaitForSeconds(0.4f);
            
#if UNITY_EDITOR
            
            action?.Invoke(ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED);
            Time.timeScale = 1;
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

                yield return new WaitForSecondsRealtime(0.5f);
                elapsed += 0.5f;
            }

            var finalStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            
            Time.timeScale = 1;
            
            action?.Invoke(finalStatus);
        }
    }
}

#endif