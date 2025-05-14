using System;
using Playbox.SdkConfigurations;

#if UNITY_EDITOR

using UnityEngine;

namespace Playbox.SdkWindow
{
    public class AppLovinWindow : DrawableWindow
    {
        private string ios_key = "";
        private string android_key = "";
        private string advertisementSdk = "";
    
        private string prev_ios_key = "";
        private string prev_android_key = "";
        private string prev_advertisementSdk = "";

        public override void InitName()
        {
            base.InitName();
            
            name = AppLovinConfiguration.Name;
        }

        public override void Body()
        {
            if (!active)
                return;
            
            prev_ios_key = ios_key;
            prev_android_key = android_key;
            prev_advertisementSdk = advertisementSdk;
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("Advertisement SDK key (Only AppLovin Integration Manager) : ");
            advertisementSdk = GUILayout.TextField(advertisementSdk, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("IOS unit id : ");
            ios_key = GUILayout.TextField(ios_key, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
        
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("Android unit id : ");
            android_key = GUILayout.TextField(android_key, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
        
            hasUnsavedChanges = !(string.Equals(ios_key, prev_ios_key, StringComparison.OrdinalIgnoreCase) && 
                                  string.Equals(prev_android_key, android_key, StringComparison.OrdinalIgnoreCase) && 
                                  string.Equals(prev_advertisementSdk, advertisementSdk, StringComparison.OrdinalIgnoreCase));
        
        }

        public override void Save()
        {
            AppLovinConfiguration.AndroidKey = android_key;
            AppLovinConfiguration.IOSKey = ios_key;
            AppLovinConfiguration.Active = active;
            AppLovinConfiguration.AdvertisementSdk = advertisementSdk;
            
            AppLovinConfiguration.SaveJsonConfig();
        }

        public override void Load()
        {
            AppLovinConfiguration.LoadJsonConfig();
        
            android_key = AppLovinConfiguration.AndroidKey;
            ios_key = AppLovinConfiguration.IOSKey;
            active = AppLovinConfiguration.Active;
            advertisementSdk = AppLovinConfiguration.AdvertisementSdk;
        
            base.Load();
        }
    }
}

#endif