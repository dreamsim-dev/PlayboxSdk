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
        public static void Build()
        {
            DebugExtentions.BeginPrefixZone("Android");
            
            var scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();
            
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
            
            PlayerSettings.Android.useCustomKeystore = true;
            EditorUserBuildSettings.buildAppBundle = SmartCma.Validations.HasStoreBuild;
            
            SetDebuggableFlag(false);
            
            if(SmartCma.Validations.HasBuildVersion) PlayerSettings.bundleVersion = SmartCma.Arguments.BundleVersion;
            if(SmartCma.Validations.HasSplashScreen) PlayerSettings.SplashScreen.showUnityLogo = SmartCma.Validations.HasSplashScreen;
            if(SmartCma.Validations.HasKeystorePass) PlayerSettings.Android.keystorePass = SmartCma.Arguments.KeystorePass;
            if(SmartCma.Validations.HasKeyaliasName) PlayerSettings.Android.keyaliasName = SmartCma.Arguments.KeyaliasName;
            if(SmartCma.Validations.HasKeyaliasPass) PlayerSettings.Android.keyaliasPass = SmartCma.Arguments.KeyaliasPass;
            if(SmartCma.Validations.HasBuildNumber) PlayerSettings.Android.bundleVersionCode = SmartCma.Arguments.BuildNumber;
            
            PlayerSettings.bundleVersion.PlayboxLog("bundleVersion");
            PlayerSettings.SplashScreen.showUnityLogo.PlayboxLog("showUnityLogo");
            PlayerSettings.Android.keystorePass.PlayboxLog("keystorePass");
            PlayerSettings.Android.keyaliasName.PlayboxLog("keyaliasName");
            PlayerSettings.Android.keyaliasPass.PlayboxLog("keyaliasPass");
            PlayerSettings.Android.bundleVersionCode.PlayboxLog("bundleVersionCode");
            
            if (SmartCma.Validations.HasKeystorePath)
            {
                PlayerSettings.Android.keystoreName = SmartCma.Arguments.KeystorePath;
                $"KeystorePath set to {SmartCma.Arguments.KeystorePath}".PlayboxInfo("Keystore Path");
            }

            if (!SmartCma.Validations.HasBuildLocation)
            {
                "No build location provided".PlayboxException("Argument Error");
            }

            DebugExtentions.EndPrefixZone();
            
            BuildOptions buildOptions = BuildOptions.None;
            
            if(SmartCma.Validations.HasDevelopmentMode)
                buildOptions = BuildOptions.Development;
                
            
            BuildPipeline.BuildPlayer(scenes, SmartCma.Arguments.BuildLocation, BuildTarget.Android, buildOptions);
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