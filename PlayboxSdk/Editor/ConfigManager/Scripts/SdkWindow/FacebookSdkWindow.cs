#if UNITY_EDITOR
using System;
using Playbox.SdkConfigurations;
using UnityEngine;

namespace Playbox.SdkWindow
{
    public class FacebookSdkWindow : DrawableWindow
    {
        private string appLabel = "";
        private string appId = "";
        private string clientToken = "";
    
        private string prev_appLabel = "";
        private string prev_appId = "";
        private string prev_clientToken = "";
        
        public override void InitName()
        {
            base.InitName();
            
            name = FacebookSdkConfiguration.Name;
        }
        
        public override void Body()
        {
            if (!active)
                return;
            
            prev_appLabel = appLabel;
            prev_appId = appId;
            prev_clientToken = clientToken;
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("app Label : ");
            appLabel = GUILayout.TextField(appLabel, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("client Token : ");
            clientToken = GUILayout.TextField(clientToken, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
            
            GUILayout.EndHorizontal();
        
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label("app id : ");
            appId = GUILayout.TextField(appId, GUILayout.ExpandWidth(false), GUILayout.Height(FieldHeight), GUILayout.Width(FieldWidth));
        
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Load from Facebook Settings"))
            {
                appId = Facebook.Unity.Settings.FacebookSettings.AppId;
                clientToken = Facebook.Unity.Settings.FacebookSettings.ClientToken;
                appLabel = Facebook.Unity.Settings.FacebookSettings.AppLabels[0];
            }
        
            hasUnsavedChanges = !(string.Equals(prev_appLabel, appLabel, StringComparison.OrdinalIgnoreCase) && 
                                  string.Equals(prev_appId, appId, StringComparison.OrdinalIgnoreCase) && 
                                  string.Equals(prev_clientToken, clientToken, StringComparison.OrdinalIgnoreCase));
        
        }

        public override void Save()
        {
            FacebookSdkConfiguration.Active = active;
            FacebookSdkConfiguration.AppLabel = appLabel;
            FacebookSdkConfiguration.AppID = appId;
            FacebookSdkConfiguration.ClientToken = clientToken;
            
            FacebookSdkConfiguration.SaveJsonConfig();
            
            base.Save();
        }

        public override void Load()
        {
            FacebookSdkConfiguration.LoadJsonConfig();
         
            active = FacebookSdkConfiguration.Active;
            appLabel = FacebookSdkConfiguration.AppLabel;
            appId = FacebookSdkConfiguration.AppID;
            clientToken = FacebookSdkConfiguration.ClientToken;
            
            base.Load();
        }
    }
}
#endif