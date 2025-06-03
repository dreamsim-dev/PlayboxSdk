using Playbox.SdkConfigurations;
using UnityEditor;
using UnityEngine;

namespace Playbox.SdkWindow
{
    public class InAppPurchaseWindow : DrawableWindow
    {
        bool useCustom = false;
        
        public override void InitName()
        {
            base.InitName();

            name = InAppConfiguration.Name;
        }

        public override void Body()
        {
            if (!active)
                return;
            
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("Use Custom In App Purchases");
            
            useCustom = EditorGUILayout.Toggle("", useCustom, GUILayout.ExpandWidth(false));
            
            GUILayout.EndHorizontal();
        }

        public override void Save()
        {
            InAppConfiguration.UseCustomInApp = useCustom;
            InAppConfiguration.Active = active;
            
            InAppConfiguration.SaveJsonConfig();
        }

        public override void Load()
        {
            InAppConfiguration.LoadJsonConfig();
            
            useCustom = InAppConfiguration.UseCustomInApp;
            active = InAppConfiguration.Active;
        }
    }
}