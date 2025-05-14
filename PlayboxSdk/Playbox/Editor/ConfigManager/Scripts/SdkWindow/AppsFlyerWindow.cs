using System;
using Playbox.SdkConfigurations;


#if UNITY_EDITOR

using UnityEngine;

namespace Playbox.SdkWindow
{
    public class AppsFlyerWindow : DrawableWindow
    {
        private string ios_key = "";
        private string android_key = "";
        private string ios_app_id = "";
        private string android_app_id = "";
        
        private string prev_ios_app_id = "";
        private string prev_android_app_id = "";
    
        private string prev_ios_version;
        private string prev_android_version;
        
        public override void InitName()
        {
            base.InitName();
            
            name = AppsFlyerConfiguration.Name;
        }
        
        public override void Body()
        {
            if (!active)
                return;
 
            prev_ios_version = ios_key;
            prev_android_version = android_key;
        
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("ios sdk key: ");
            ios_key = GUILayout.TextField(ios_key, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("ios app id : ");
            ios_app_id = GUILayout.TextField(ios_app_id, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
        
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("android sdk key: ");
            android_key = GUILayout.TextField(android_key, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("android app id : ");
            android_app_id = GUILayout.TextField(android_app_id, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
        
            hasUnsavedChanges = !(string.Equals(prev_ios_version, ios_key, StringComparison.OrdinalIgnoreCase) &&
                                  string.Equals(prev_android_version, android_key, StringComparison.OrdinalIgnoreCase) &&
                                  string.Equals(prev_ios_app_id, ios_app_id, StringComparison.OrdinalIgnoreCase) &&
                                  string.Equals(prev_android_app_id, android_app_id, StringComparison.OrdinalIgnoreCase)
                                  );
        }

        public override void Save()
        {
            AppsFlyerConfiguration.AndroidKey = ios_key;
            AppsFlyerConfiguration.IOSKey = android_key;
            AppsFlyerConfiguration.Active = active;
            AppsFlyerConfiguration.IOSAppId = ios_app_id;
            AppsFlyerConfiguration.AndroidAppId = android_app_id;
        
            AppsFlyerConfiguration.SaveJsonConfig();
        }

        public override void Load()
        {
            AppsFlyerConfiguration.LoadJsonConfig();
        
            ios_key = AppsFlyerConfiguration.AndroidKey;
            android_key = AppsFlyerConfiguration.IOSKey;
            active = AppsFlyerConfiguration.Active;
            ios_app_id = AppsFlyerConfiguration.IOSAppId;
            android_app_id = AppsFlyerConfiguration.AndroidAppId;
        
            base.Load();
        }
    }
}

#endif