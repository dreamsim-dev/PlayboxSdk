namespace Playbox.CI
{
    public static class SmartCma{
        
        public static class Arguments
        {
            public static string BuildLocation => SmartEnviroment.GetArgumentValue(Constants.BUILD_LOCATION);
            public static string BuildVersion => SmartEnviroment.GetArgumentValue(Constants.BUILD_VERSION);
            public static string BundleVersion => BuildVersion;
            public static int BuildNumber => SmartEnviroment.GetArgumentIntValue(Constants.BUILD_NUMBER);
            public static string KeystorePass => SmartEnviroment.GetArgumentValue(Constants.KEYSTORE_PASS);
            public static string KeyaliasName => SmartEnviroment.GetArgumentValue(Constants.KEYALIAS_NAME);
            public static string KeyaliasPass => SmartEnviroment.GetArgumentValue(Constants.KEYALIAS_PASS);
            public static string KeystorePath => SmartEnviroment.GetArgumentValue(Constants.KEYSTORE_PATH);
            public static string ProvisionProfileIos => SmartEnviroment.GetArgumentValue(Constants.PROVISION_PROFILE_IOS_SIGN);
            public static string CodeSignIdentity => SmartEnviroment.GetArgumentValue(Constants.CODE_SIGN_IDENTITY);
        }
        
        public static class Validations
        {
            public static bool HasDevelopmentMode => SmartEnviroment.HasArgument(Constants.DEVELOPMENT_MODE);
            public static bool HasBuildLocation => SmartEnviroment.HasArgument(Constants.BUILD_LOCATION);
            public static bool HasSplashScreen => SmartEnviroment.HasArgument(Constants.SPLASH_SCREEN);
            public static bool HasBuildVersion => SmartEnviroment.HasArgument(Constants.BUILD_VERSION);
            public static bool HasBuildNumber => SmartEnviroment.HasArgument(Constants.BUILD_NUMBER);
            public static bool HasKeystorePass => SmartEnviroment.HasArgument(Constants.KEYSTORE_PASS);
            public static bool HasKeyaliasName => SmartEnviroment.HasArgument(Constants.KEYALIAS_NAME);
            public static bool HasKeyaliasPass => SmartEnviroment.HasArgument(Constants.KEYALIAS_PASS);
            public static bool HasKeystorePath => SmartEnviroment.HasArgument(Constants.KEYSTORE_PATH);
            public static bool HasStoreBuild => SmartEnviroment.HasArgument(Constants.STORE_BUILD);
            public static bool HasIosManualSign => SmartEnviroment.HasArgument(Constants.MANAUL_SIGN);
            public static bool HasProvisionProfileIos => SmartEnviroment.HasArgument(Constants.PROVISION_PROFILE_IOS_SIGN);
            public static bool HasCodeSignIdentity => SmartEnviroment.HasArgument(Constants.CODE_SIGN_IDENTITY);
            public static bool HasProfileDevelopment => SmartEnviroment.HasArgument(Constants.PROFILE_DEVELOPMENT);
            public static bool HasProfileDistribution => SmartEnviroment.HasArgument(Constants.PROFILE_DISTRIBUTION);
        }
        
        public static class Constants
        {
            public const string DEVELOPMENT_MODE = "-debug"; // Дебаг мод
            public const string BUILD_LOCATION = "-build-location"; // куда выгружается билд
            public const string SPLASH_SCREEN = "-splash-screen"; // Показываем ли всплывающий экран
            public const string BUILD_VERSION = "-build-version"; // appVersion по типу 0.0.1
            public const string BUILD_NUMBER = "-build-number"; // номер билда в лденкинс или подобном пример 179
            public const string KEYSTORE_PASS = "-keystorepass"; 
            public const string KEYALIAS_NAME = "-keyaliasname";
            public const string KEYALIAS_PASS = "-keyaliaspass";
            public const string KEYSTORE_PATH = "-keystore-path";
            public const string STORE_BUILD = "-store-build"; 
            
            public const string MANAUL_SIGN = "-code-sign-manual";
            public const string PROVISION_PROFILE_IOS_SIGN = "-provision-profile";
            public const string CODE_SIGN_IDENTITY = "-code-sign-identity";
            public const string PROFILE_DEVELOPMENT = "-profile-development";
            public const string PROFILE_DISTRIBUTION = "-profile-distribution";
        }
    }
}