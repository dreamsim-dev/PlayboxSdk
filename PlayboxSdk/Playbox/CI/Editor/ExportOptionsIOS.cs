using UnityEditor.iOS.Xcode;

namespace Playbox.CI
{
    public class ExportOptionsIOS
    {
        public string BuildVersion { get; set; } = "0.0.0";
        public string DocumentVersion { get; set; } = "0.1";
        public string ExportOptionsFileName => deployPlistName;

        private const string deployPlistName = "exportOptions.plist";

        private PlistDocument document;

        public ExportOptionsIOS()
        {
            document = new PlistDocument();
            
            document.version = DocumentVersion;
            document.root.SetString("method","app-store-connect");
            document.root.SetBoolean("uploadSymbols",true);
            document.root.SetBoolean("uploadBitcode",false);
            document.root.SetString("destination","upload");
            document.root.SetString("appVersion",BuildVersion);
            
            
            // Ручная подпись
            document.root.SetString("signingStyle",        "manual");
            document.root.SetString("signingCertificate",  "Apple Distribution: APS DATA LLC (AD5J7QFQ52)");
            document.root.SetString("teamID",              "AD5J7QFQ52");

            // Привязка provisioning profile к bundle identifier
            var profiles = document.root.CreateDict("provisioningProfiles");
            profiles.SetString(
                "world.dreamsim.ragstoriches",   // ваш Bundle ID
                "rags-to-riches"                 // имя provisioning profile
            );
        }

        public override string ToString()
        {
            return document.WriteToString();
        }
    }
}