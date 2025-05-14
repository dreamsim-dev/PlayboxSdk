using System;
using Firebase;
using Firebase.Crashlytics;
using Firebase.Extensions;
using UnityEngine;

namespace Playbox
{
    public class FirebaseInitialization : PlayboxBehaviour
    {
        public override void Initialization()
        {
            base.Initialization();
            
            InitializeCrashlytics();
 
        }

        public static void InitializeCrashlytics()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    Init();
                }
                else
                {
                    Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        private static void Init()
        {
            Crashlytics.ReportUncaughtExceptionsAsFatal = true; 
            Crashlytics.IsCrashlyticsCollectionEnabled = true;
        }

        public static void LogCrashlytics(string message)
        {
            Crashlytics.Log(message); 
        }
        
        public static void LogCrashlyticsException(string message)
        {
            Crashlytics.LogException(new Exception(message));
        }
    }
}
