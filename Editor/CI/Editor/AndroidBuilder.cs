using System.Linq;
using CI.Utils.Extentions;

#if UNITY_ANDROID && UNITY_EDITOR
using Facebook.Unity;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Playbox.CI
{
    public static class Android
    {
        [MenuItem("PlayBox/Builder/Android Build")]
        public static void Build()
        {
            DebugExtentions.BeginPrefixZone("Android");
            
            var scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
            
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            bool debug = SmartCLA.Validations.HasDevelopmentMode;

            EditorUserBuildSettings.development = debug;

            if (debug)
            {
                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Debug;
            }
            else
            {
                EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
            }

            PlayerSettings.Android.useCustomKeystore = true;
            EditorUserBuildSettings.buildAppBundle = SmartCLA.Validations.HasStoreBuild;
            
            SetDebuggableFlag(debug);
            
            if(SmartCLA.Validations.HasBuildVersion) PlayerSettings.bundleVersion = SmartCLA.Arguments.BuildVersion;
            if(SmartCLA.Validations.HasSplashScreen) PlayerSettings.SplashScreen.showUnityLogo = SmartCLA.Validations.HasSplashScreen;
            if(SmartCLA.Validations.HasKeystorePass) PlayerSettings.Android.keystorePass = SmartCLA.Arguments.KeystorePass;
            if(SmartCLA.Validations.HasKeyaliasName) PlayerSettings.Android.keyaliasName = SmartCLA.Arguments.KeyaliasName;
            if(SmartCLA.Validations.HasKeyaliasPass) PlayerSettings.Android.keyaliasPass = SmartCLA.Arguments.KeyaliasPass;
            if(SmartCLA.Validations.HasBuildNumber) PlayerSettings.Android.bundleVersionCode = SmartCLA.Arguments.BuildNumber;
            
            PlayerSettings.bundleVersion.PlayboxLog("bundleVersion");
            PlayerSettings.SplashScreen.showUnityLogo.PlayboxLog("showUnityLogo");
            PlayerSettings.Android.keystorePass.PlayboxLog("keystorePass");
            PlayerSettings.Android.keyaliasName.PlayboxLog("keyaliasName");
            PlayerSettings.Android.keyaliasPass.PlayboxLog("keyaliasPass");
            PlayerSettings.Android.bundleVersionCode.PlayboxLog("bundleVersionCode");
            
            if (SmartCLA.Validations.HasKeystorePath)
            {
                PlayerSettings.Android.keystoreName = SmartCLA.Arguments.KeystorePath;
                $"KeystorePath set to {SmartCLA.Arguments.KeystorePath}".PlayboxInfo("Keystore Path");
            }

            if (!SmartCLA.Validations.HasBuildLocation)
            {
                "No build location provided".PlayboxException("Argument Error");
            }

            DebugExtentions.EndPrefixZone();
            
            BuildOptions buildOptions = BuildOptions.None;
            
            if(SmartCLA.Validations.HasDevelopmentMode)
                buildOptions = BuildOptions.Development;
                
            
            BuildPipeline.BuildPlayer(scenes, SmartCLA.Arguments.BuildLocation, BuildTarget.Android, buildOptions);
        }
        
        private static void SetDebuggableFlag(bool enabled)
        {
            string manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
            
            if (!System.IO.File.Exists(manifestPath))
            {
                "Unity will generate the file automatically.".PlayboxWarning("AndroidManifest.xml not found in project folder.");
                return;
            }
            
            string manifestText = System.IO.File.ReadAllText(manifestPath);
            
            if (manifestText.Contains("android:debuggable="))
            {
                manifestText = System.Text.RegularExpressions.Regex.Replace(
                    manifestText,
                    @"android:debuggable=""(true|false)""",
                    $"android:debuggable=\"{enabled.ToString().ToLower()}\""
                );
            }
            else
            {
               
                manifestText = manifestText.Replace(
                    "<application ",
                    $"<application android:debuggable=\"{enabled.ToString().ToLower()}\" "
                );
            }
    
            "AndroidManifest.xml changed".PlayboxLog($"debuggable flag changed to {enabled.ToString().ToLower()}.");
        
            System.IO.File.WriteAllText(manifestPath, manifestText);
            AssetDatabase.Refresh();
        }
     
    }
}

#endif