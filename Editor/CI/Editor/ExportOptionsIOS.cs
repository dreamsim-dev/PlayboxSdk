#if UNITY_EDITOR && UNITY_IOS

using UnityEditor.iOS.Xcode;

namespace Playbox.CI
{
    public class ExportOptionsIOS
    {
        public string BuildVersion { get; set; } = "0.0.0";
        public string DocumentVersion { get; set; } = "0.1";
        public string ExportOptionsFileName => DeployPlistName;

        private const string DeployPlistName = "exportOptions.plist";

        private PlistDocument document;

        public ExportOptionsIOS()
        {
            document = new PlistDocument();
            
            document.version = DocumentVersion;
            document.root.SetString("method","app-store");
            document.root.SetBoolean("uploadSymbols",true);
            document.root.SetBoolean("uploadBitcode",false);
            document.root.SetString("destination","upload");
            document.root.SetString("appVersion",BuildVersion);

            if (SmartCLA.Validations.HasIosManualSign)
            {
                document.root.SetString("signingStyle", "manual");
            }
            
            document.root.SetString("signingCertificate", "Apple Distribution: APS DATA LLC (AD5J7QFQ52)");
            document.root.SetString("teamID", "AD5J7QFQ52");
            
            var profiles = document.root.CreateDict("provisioningProfiles");
            
            profiles.SetString(Playbox.Data.Playbox.GameId, SmartCLA.Arguments.ProvisionProfileIos);
        }

        public override string ToString()
        {
            return document.WriteToString();
        }
    }
}

#endif