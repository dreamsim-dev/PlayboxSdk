#if UNITY_EDITOR

using System.IO;
using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;
using Playbox.SdkConfigurations;
using UnityEditor;
using UnityEngine;

namespace Playbox
{
    public static class PlayboxMultiSceneInstaller
    {
        private const string flagKey = "playbox_helper";
        
        [InitializeOnEnterPlayMode]
        static void InstallPlaybox()
        {
            bool enabled = EditorPrefs.GetBool(flagKey, true);
            
            if(!enabled)
                return;
            
            EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    var mainInit = Object.FindFirstObjectByType<MainInitialization>();

                    if (mainInit == null)
                    {
                        var go = new GameObject("Playbox SDK Installers");
              
                        go.AddComponent<MainInitialization>();  
              
                    }
                }
            };
        }

        [MenuItem("PlayBox/SceneHelper/Enable Helper")]
        public static void ToggleHelper()
        {
            bool enabled = !EditorPrefs.GetBool(flagKey, true);
            
            
            if(enabled)
                "Enable Editor Helper".PlayboxInfo("HELPER");
            else
                "Disable Editor Helper".PlayboxInfo("HELPER");
            
            EditorPrefs.SetBool(flagKey, enabled);
        }

        [MenuItem("PlayBox/SceneHelper/Enable Helper", true)]
        public static bool ToggleHelperValidate()
        {
            bool enabled = EditorPrefs.GetBool(flagKey, true);
            
            Menu.SetChecked("PlayBox/SceneHelper/Enable Helper", enabled);
            
            return true;
        }
        
    }  
}

#endif