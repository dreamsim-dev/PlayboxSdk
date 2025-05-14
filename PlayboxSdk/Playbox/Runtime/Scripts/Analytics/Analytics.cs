using System.Collections.Generic;
using System.Linq;
using DevToDev.Analytics;
using AppsFlyerSDK;
using Firebase.Analytics;
using UnityEngine;

namespace Playbox
{
    public static class Analytics
    {
        public static void TrackEvent(string eventName, List<KeyValuePair<string,string>> arguments)
        {
           DTDAnalytics.CustomEvent(eventName, arguments.ToCustomParameters());
           
           AppsFlyer.sendEvent(eventName, arguments.ToDictionary(a => a.Key, a => a.Value));
           
           FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void TrackEvent(string eventName)
        {
            AppsFlyer.AFLog(nameof(Analytics.TrackEvent), eventName);
            
            FirebaseAnalytics.LogEvent(eventName);
        }
        
        public static void TrackSimpleEvent(string eventName, string value)
        {
            TrackEvent(eventName, new List<KeyValuePair<string,string>> {new(eventName, value)});
        }

        public static void Log(string message)
        {
            FirebaseAnalytics.LogEvent(message);
        }
        
        public static void LogError(string error)
        {
           Firebase.Crashlytics.Crashlytics.Log(error);
        }
    }
}