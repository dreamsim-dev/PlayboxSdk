﻿#if UNITY_EDITOR && UNITY_IOS

using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Playbox.CI
{
    public class BuildProcessorApple : MonoBehaviour
    {
        private const string EntitlementsFileName =
            "Entitlements.entitlements";

        [MenuItem("PlayBox/Generate Apple")]
        public static void GenerateApplePodfile()
        {
            var path = Path.Combine(Application.dataPath,"ExternalDependencyManager","Editor");
            
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var pathFile = Path.Combine(path, "Dependencies.xml");

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                NewLineOnAttributes = false
            };
            
            using (XmlWriter writer = XmlWriter.Create(pathFile, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("dependencies");

                writer.WriteStartElement("iosPods");

                writer.WriteStartElement("iosPod");
                writer.WriteAttributeString("name", "Google-Mobile-Ads-SDK");
                writer.WriteAttributeString("version", "12.5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("iosPod");
                writer.WriteAttributeString("name", "AppLovinMediationGoogleAdapter");
                writer.WriteAttributeString("version", "12.5.0.0");
                writer.WriteEndElement();

                writer.WriteStartElement("iosPod");
                writer.WriteAttributeString("name", "AppLovinMediationGoogleAdManagerAdapter");
                writer.WriteAttributeString("version", "12.5.0.0");
                writer.WriteEndElement();

                writer.WriteEndElement(); 
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            
            
            AssetDatabase.Refresh();

            Debug.Log("Dependencies.xml succes generated!");

        }

        [PostProcessBuild(999)]
        public static void DoPostProcess(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS)
                return; 
            
            EditorUserBuildSettings.symlinkSources = false;
            EditorUserBuildSettings.buildAppBundle = false;
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

            if (SmartCma.Validations.HasIosManualSign)
            {
                project.SetBuildProperty(mainTarget, "CODE_SIGN_STYLE", "Manual");
                project.SetBuildProperty(unityFrameworkTarget, "CODE_SIGN_STYLE", "Manual");
            }

            if (SmartCma.Validations.HasProvisionProfileIos)
            {
                project.SetBuildProperty(mainTarget, "PROVISIONING_PROFILE_SPECIFIER", SmartCma.Arguments.ProvisionProfileIos);
            }
            
            if (SmartCma.Validations.HasCodeSignIdentity)
            {
                project.SetBuildProperty(mainTarget, "CODE_SIGN_IDENTITY", SmartCma.Arguments.CodeSignIdentity);
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
            var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
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
            var projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            
            proj.ReadFromFile(projPath);
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
    }
}
#endif