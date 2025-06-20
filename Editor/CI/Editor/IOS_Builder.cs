#if UNITY_EDITOR && UNITY_IOS

using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using CI.Utils.Extentions;
using UnityEditor.iOS.Xcode;

namespace Playbox.CI
{
    public static class IOS
    {
        public const string TeamID = "AD5J7QFQ52";
        
        [UsedImplicitly]
        public static void Build()
        {
            var scenes = EditorBuildSettings.scenes.Select(x => x.path)
                .ToArray();
         
            PlayerSettings.iOS.buildNumber = SmartCma.Arguments.BuildNumber.ToString();
            PlayerSettings.bundleVersion = SmartCma.Arguments.BuildVersion;
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = false;

            PlayerSettings.iOS.appleDeveloperTeamID = SmartCma.Arguments.TeamID ?? TeamID; // for testing

            if (!SmartCma.Validations.HasBuildLocation)
            {
                "No build location provided".PlayboxException();
            }

            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Automatic;
            
            if (SmartCma.Validations.HasProfileDevelopment)
            {
                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Development;
            }
            if (SmartCma.Validations.HasProfileDistribution)
            {
                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
            }
            
            PlayerSettings.iOS.appleEnableAutomaticSigning = !SmartCma.Validations.HasIosManualSign;
            
            BuildOptions buildOptions = BuildOptions.None;
            
            if(SmartCma.Validations.HasDevelopmentMode)
                buildOptions = BuildOptions.Development;
            
            BuildPipeline.BuildPlayer(scenes, SmartCma.Arguments.BuildLocation, BuildTarget.iOS, buildOptions);
        }

        [PostProcessBuild]
        public static void CopyExportOptionsPlist(BuildTarget target,
            string path)
        {
            if (target != BuildTarget.iOS)
                return; 
            
            ExportOptionsIOS exportOptionsIOS = new ExportOptionsIOS
            {
                BuildVersion = SmartCma.Arguments.BuildVersion
            };

            var deployPlistDestination = Path.Combine(path, exportOptionsIOS.ExportOptionsFileName);
            
            $"[PlayBox] Export options file path: {deployPlistDestination}".PlayboxLog();
            
            File.WriteAllText(deployPlistDestination, exportOptionsIOS.ToString());
        }

        [PostProcessBuild]
        public static void SetNonExemptEncryptionKey(BuildTarget target,
            string path)
        {
            if (target != BuildTarget.iOS)
                return; 
            
            var plistPath = Path.Combine(path, "Info.plist");
            
            var plist = new PlistDocument();
            
            plist.ReadFromFile(plistPath);
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif
