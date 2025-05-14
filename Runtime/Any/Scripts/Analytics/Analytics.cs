using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using DevToDev.Analytics;
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
        
        public static void TrackEvent(string eventName, KeyValuePair<string,string> eventPair)
        {
            var arguments = new Dictionary<string,string>();
            arguments.Add(eventPair.Key, eventPair.Value);
            
            DTDAnalytics.CustomEvent(eventName, arguments.ToList().ToCustomParameters());
           
            AppsFlyer.sendEvent(eventName, arguments);
           
            FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void LogLevelUp(int level)
        {
            DTDAnalytics.LevelUp(level);
        }
        
        public static void LogContentView(string content)
        {
            TrackEvent(nameof(LogContentView),new KeyValuePair<string, string>(nameof(LogContentView),content));
        }

        public static void LogTutorialSkipped(string tutorial)
        {
            TrackEvent("TutorialSkipped");
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