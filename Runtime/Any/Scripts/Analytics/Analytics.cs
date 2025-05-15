using System;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using DevToDev.Analytics;
using Facebook.Unity;
using Firebase.Analytics;
using UnityEngine;

namespace Playbox
{
    public enum ETutorialState
    {
        Start,
        Skipped,
        Complete,
    }

    public static class Analytics
    {
        public static void TrackEvent(string eventName, List<KeyValuePair<string,string>> arguments)
        {
           DTDAnalytics.CustomEvent(eventName, arguments.ToCustomParameters());
           
           AppsFlyer.sendEvent(eventName, arguments.ToDictionary(a => a.Key, a => a.Value));
           
           FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
           
           FB.LogAppEvent(eventName,null,arguments.ToDictionary(a => a.Key, a => (object)a.Value));
           
        }
        
        public static void TrackEvent(string eventName, KeyValuePair<string,string> eventPair)
        {
            var arguments = new Dictionary<string,string>();
            arguments.Add(eventPair.Key, eventPair.Value);
            
            DTDAnalytics.CustomEvent(eventName, arguments.ToList().ToCustomParameters());
            
            FB.LogAppEvent(eventName,null,arguments.ToDictionary(a => a.Key, a => (object)a.Value));
           
            AppsFlyer.sendEvent(eventName, arguments);
           
            FirebaseAnalytics.LogEvent(eventName,new Parameter(eventName,JsonUtility.ToJson(arguments)));
        }

        public static void LogLevelUp(int level)
        {
            DTDAnalytics.LevelUp(level);
            TrackEvent("LevelUp",new KeyValuePair<string, string>("level",level.ToString()));
        }
        
        public static void LogContentView(string content)
        {
            TrackEvent(nameof(LogContentView),new KeyValuePair<string, string>(nameof(LogContentView),content));
        }

        public static void LogTutorial(string tutorial, ETutorialState stateLevel = ETutorialState.Complete)
        {
            switch (stateLevel)
            {
                case ETutorialState.Start:
                    TrackEvent("tutorial",new KeyValuePair<string, string>("tutorial","start"));
                    break;
                
                case ETutorialState.Skipped:
                    TrackEvent("tutorial",new KeyValuePair<string, string>("tutorial","skip"));
                    break;
                
                case ETutorialState.Complete:
                    TrackEvent("tutorial",new KeyValuePair<string, string>("tutorial","complete"));
                    break;
                
                default:
                    TrackEvent("tutorial",new KeyValuePair<string, string>("tutorial","completed"));
                    break;
            }
        }

        public static void TrackEvent(string eventName)
        {
            AppsFlyer.AFLog(nameof(Analytics.TrackEvent), eventName);
            
            FirebaseAnalytics.LogEvent(eventName);
            
            FB.LogAppEvent(eventName);
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