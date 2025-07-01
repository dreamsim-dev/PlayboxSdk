#if UNITY_EDITOR && UNITY_IOS

using System;
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
         
            PlayerSettings.iOS.buildNumber = SmartCLA.Arguments.BuildNumber.ToString();
            PlayerSettings.bundleVersion = SmartCLA.Arguments.BuildVersion;
            
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = false;

            PlayerSettings.iOS.appleDeveloperTeamID = SmartCLA.Arguments.TeamID ?? TeamID;

            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Automatic;
            
            PlayerSettings.SplashScreen.showUnityLogo = false;
            
            if (SmartCLA.Validations.HasProfileDevelopment)
            {
                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Development;
            }
            if (SmartCLA.Validations.HasProfileDistribution)
            {
                PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
            }
            
            PlayerSettings.iOS.appleEnableAutomaticSigning = !SmartCLA.Validations.HasIosManualSign;
            
            BuildOptions buildOptions = BuildOptions.None;
            
            if(SmartCLA.Validations.HasDevelopmentMode)
                buildOptions = BuildOptions.Development;
            
            
            if (SmartCLA.Validations.HasBuildLocation)
            {
                BuildPipeline.BuildPlayer(scenes, SmartCLA.Arguments.BuildLocation, BuildTarget.iOS, buildOptions);
            }
            else
            {
                throw new Exception("Assembly path is not specified");
            }
        }
        
    }
}
#endif
