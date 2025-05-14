using System;
using Playbox.SdkConfigurations;

#if UNITY_EDITOR
using System.Collections.Generic;
using DevToDev.Analytics;
using UnityEditor;
using UnityEngine;

namespace Playbox.SdkWindow
{
    public class DevToDevWindow : DrawableWindow
    {
        private string ios_key = "";
        private string android_key = "";
    
        private string prev_ios_version = "";
        private string prev_android_version = "";
        
        DTDLogLevel         logLevel   = 0;
        private List<string> _options = new();
        
        public override void InitName()
        {
            base.InitName();
            
            _options.Clear();

            foreach (var item in Enum.GetNames(typeof(DTDLogLevel)))
            {
                _options.Add(item);
            }
            
            
            name = DevToDevConfiguration.Name;
        }

        public override void Header()
        {
            base.Header();

            GUILayout.BeginVertical();
        }

        public override void Footer()
        {
            base.Footer();
            
            GUILayout.EndVertical();
        }

        public override void Body()
        {
            if (!active)
                return;
            
            prev_ios_version = ios_key;
            prev_android_version = android_key;

            logLevel = (DTDLogLevel)EditorGUILayout.Popup("LogLevel", (int)logLevel, _options.ToArray(), GUILayout.Width(300));

            GUILayout.BeginHorizontal();
        
            GUILayout.Label("IOS : ");
            ios_key = GUILayout.TextField(ios_key, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
        
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("Android : ");
            android_key = GUILayout.TextField(android_key, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            hasUnsavedChanges = !(string.Equals(prev_ios_version, ios_key, StringComparison.OrdinalIgnoreCase) &&
                                  string.Equals(prev_android_version, android_key, StringComparison.OrdinalIgnoreCase));
        
        }

        public override void Save()
        {
            DevToDevConfiguration.AndroidKey = android_key;
            DevToDevConfiguration.IOSKey = ios_key;
            DevToDevConfiguration.Active = active;
            DevToDevConfiguration.LOGLevel = logLevel;
            
            DevToDevConfiguration.SaveJsonConfig();
        }

        public override void Load()
        {
            DevToDevConfiguration.LoadJsonConfig();
        
            android_key = DevToDevConfiguration.AndroidKey;
            ios_key = DevToDevConfiguration.IOSKey;
            active = DevToDevConfiguration.Active;
            logLevel = DevToDevConfiguration.LOGLevel;
        
            base.Load();
        }
    }
}

#endif