#if UNITY_EDITOR && UNITY_IOS

using System.IO;
using CI.Utils.Extentions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Playbox.CI
{
    public class AppleBuildPostProcessor : MonoBehaviour
    {
        private const string EntitlementsFileName =
            "Entitlements.entitlements";

        [PostProcessBuild(0)]
        public static void SetBuildSettings(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
                return; 
            
            EditorUserBuildSettings.symlinkSources = false;
            EditorUserBuildSettings.buildAppBundle = false;
        }

        [PostProcessBuild(999)]
        public static void DoPostProcess(BuildTarget target, string path)
        {
            
            AddCapabilities(path);
            ProcessPlist(path);
            FixFirebase(path);
        }

        [PostProcessBuild]
        public static void UpdateUploadToken(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return; 
            
            var pbxFilename =
                path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var project = new PBXProject();
            project.ReadFromFile(pbxFilename);

            var targetGuid =
                project.GetUnityMainTargetGuid();
            var token =
                project.GetBuildPropertyForAnyConfig(targetGuid,
                    "USYM_UPLOAD_AUTH_TOKEN");

            if (string.IsNullOrEmpty(token)) token = "FakeToken";

            project.SetBuildProperty(targetGuid,
                "USYM_UPLOAD_AUTH_TOKEN",
                token);
            project.WriteToFile(pbxFilename);
        }

        [PostProcessBuild]
        public static void ConfigureXcode(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;
            
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);
            
            string mainTarget = project.GetUnityMainTargetGuid();
            string unityFrameworkTarget = project.GetUnityFrameworkTargetGuid();
            
            project.AddBuildProperty(mainTarget, "LIBRARY_SEARCH_PATHS", "$(inherited)");
            project.AddBuildProperty(unityFrameworkTarget, "LIBRARY_SEARCH_PATHS", "$(inherited)");
            project.AddBuildProperty(mainTarget, "SWIFT_OPTIMIZATION_LEVEL", "-Onone");
            project.AddBuildProperty(unityFrameworkTarget, "SWIFT_OPTIMIZATION_LEVEL", "-Onone");
            
            string iosVersion = "15.0"; 
            project.SetBuildProperty(mainTarget, "IPHONEOS_DEPLOYMENT_TARGET", iosVersion);
            project.SetBuildProperty(unityFrameworkTarget, "IPHONEOS_DEPLOYMENT_TARGET", iosVersion);

            project.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            project.SetBuildProperty(mainTarget, "CLANG_ENABLE_MODULES", "YES");
            project.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");

            if (SmartCLA.Validations.HasIosManualSign)
            {
                project.SetBuildProperty(mainTarget, "CODE_SIGN_STYLE", "Manual");
                project.SetBuildProperty(unityFrameworkTarget, "CODE_SIGN_STYLE", "Manual");
            }

            if (SmartCLA.Validations.HasProvisionProfileIos)
            {
                project.SetBuildProperty(mainTarget, "PROVISIONING_PROFILE_SPECIFIER", SmartCLA.Arguments.ProvisionProfileIos);
            }
            
            if (SmartCLA.Validations.HasCodeSignIdentity)
            {
                project.SetBuildProperty(mainTarget, "CODE_SIGN_IDENTITY", SmartCLA.Arguments.CodeSignIdentity);
            }
            
            File.WriteAllText(projectPath, project.WriteToString());
        }

        [PostProcessBuild]
        public static void RemoveDuplicateFiles(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;

            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            string targetGuid = project.GetUnityFrameworkTargetGuid();

            project.SetBuildProperty(targetGuid, "GCC_TREAT_WARNINGS_AS_ERRORS", "NO");
            project.SetBuildProperty(targetGuid, "DEVELOPMENT_TEAM", Playbox.CI.IOS.TeamID);

            File.WriteAllText(projectPath, project.WriteToString());
        }

        private static void AddCapabilities(string path)
        {
            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            var mainTargetGuid = project.GetUnityMainTargetGuid();
            var manager = new ProjectCapabilityManager(projectPath,
                EntitlementsFileName,
                null,
                mainTargetGuid);
            manager.AddPushNotifications(false);
            manager.WriteToFile();
        }

        private static void ProcessPlist(string path)
        {
            var plistPath = path + "/Info.plist";
            var plist = new PlistDocument();
            
            plist.ReadFromFile(plistPath);
            
            var nsAppTransportSecurity = plist.root["NSAppTransportSecurity"];
            
            nsAppTransportSecurity.AsDict().values.Remove("NSAllowsArbitraryLoadsInWebContent");
            
            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void FixFirebase(string path)
        {
            var filePath = "GoogleService-Info.plist";
            
            var projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            
            proj.ReadFromFile(projPath);
            
            if(!File.Exists(filePath))
                throw new FileNotFoundException("Could not find file: " + filePath);
            
            proj.AddFileToBuild(proj.GetUnityMainTargetGuid(), proj.AddFile("GoogleService-Info.plist", "GoogleService-Info.plist"));
            proj.WriteToFile(projPath);
        }
        
        [PostProcessBuild(100)]
        public static void GenerateReleaseEntitlements(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;

            string entitlementsPath = Path.Combine(path, "Unity-iPhone/Release.entitlements");
            PlistDocument entitlements = new PlistDocument();
            PlistElementDict rootDict = entitlements.root;
            
            rootDict.SetBoolean("get-task-allow", false); 
            rootDict.SetString("aps-environment", "production"); 
            
            PlistElementArray keychainGroups = rootDict.CreateArray("keychain-access-groups");
            keychainGroups.AddString($"$(AppIdentifierPrefix){PlayerSettings.applicationIdentifier}");

            File.WriteAllText(entitlementsPath, entitlements.WriteToString());
            
            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(path);
            project.ReadFromFile(projectPath);

            var targetGuid = project.GetUnityMainTargetGuid();
            
            project.SetBuildPropertyForConfig(targetGuid, "Release", "CODE_SIGN_ENTITLEMENTS");
            project.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS[config=Release]", "Unity-iPhone/Release.entitlements");
            
            project.WriteToFile(projectPath);
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
            
            const string trackingKey = "NSUserTrackingUsageDescription";
            const string trackingMessage = "This identifier helps us track app installs and show personalized ads";

            plist.root.SetString(trackingKey, trackingMessage);
            
            File.WriteAllText(plistPath, plist.WriteToString());
        }
        
        [PostProcessBuild]
        public static void CopyExportOptionsPlist(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return; 
            
            ExportOptionsIOS exportOptionsIOS = new ExportOptionsIOS
            {
                BuildVersion = SmartCLA.Arguments.BuildVersion
            };

            var deployPlistDestination = Path.Combine(path, exportOptionsIOS.ExportOptionsFileName);
            File.WriteAllText(deployPlistDestination, exportOptionsIOS.ToString());
        }
    }
}
#endif