using System.Collections.Generic;
using Playbox.SdkConfigurations;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Playbox.SdkWindow
{
    public class ConfigurationWindow : EditorWindow
    {
        DrawableWindow[] drawableWindow;
    
        List<DrawableWindow> drawableWindowList = new();
    
        ConfigurationWindow configurationWindow;
        
        Vector2 scrollPosition;
    
        [MenuItem("PlayBox/Configuration")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConfigurationWindow>("Playbox Configuration");
            window.hasUnsavedChanges = true;
        }

        private void CreateGUI()
        {
            drawableWindowList.Add(new AppsFlyerWindow());
            drawableWindowList.Add(new DevToDevWindow());
            drawableWindowList.Add(new AppLovinWindow());
            drawableWindowList.Add(new FacebookSdkWindow());
            drawableWindowList.Add(new InAppPurchaseWindow());
        
            GlobalPlayboxConfig.Load();
        
            foreach (var item in drawableWindowList)
            {
                item.InitName();
                item.Load();
                item.FieldHeight = EditorGUIUtility.singleLineHeight;
                item.FieldWidth = 450;
            }
        
            hasUnsavedChanges = false;
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Space(20f);
            
            GUILayout.Label(titleContent, EditorStyles.boldLabel);
        
            GUILayout.Space(10f);

            foreach (var item in drawableWindowList)
            {
                item.HasRenderToggle();
                
                item.Title();
                item.Header();
                item.Body();
                item.Footer();

                hasUnsavedChanges = hasUnsavedChanges || item.hasUnsavedChanges;
            }

            if (GUILayout.Button("Save Configuration"))
            {
                GlobalPlayboxConfig.Clear();
            
                foreach (var item in drawableWindowList)
                {
                    item.Save();
                }
            
                GlobalPlayboxConfig.Save();
            
                hasUnsavedChanges = false;
            }
            
            GUILayout.EndScrollView();
        }

        public override void SaveChanges()
        {
            GlobalPlayboxConfig.Clear();
            
            foreach (var item in drawableWindowList)
            {
                item.Save();
            }
            
            GlobalPlayboxConfig.Save();

            base.SaveChanges();
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
        }
    }
}

#endif