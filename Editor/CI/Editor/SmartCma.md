# SmartCma Documentation

Playbox CI static helper for build arguments, validations, and constants.

---

## SmartCma.Arguments

Properties for retrieving command-line argument values:
- **BuildLocation**: Build output location
- **BuildVersion**: App version (e.g., "0.0.1")
- **BundleVersion**: Alias for BuildVersion
- **BuildNumber**: Build number from CI/CD (e.g., Jenkins ID)
- **KeystorePass**: Keystore password
- **KeyaliasName**: Key alias name
- **KeyaliasPass**: Key alias password
- **KeystorePath**: Keystore file path
- **ProvisionProfileIos**: iOS provisioning profile path
- **CodeSignIdentity**: iOS code signing identity

---

## SmartCma.Validations

Properties to check presence of specific arguments:
- **HasDevelopmentMode**: Checks if debug mode is enabled
- **HasBuildLocation**: Checks if build location is specified
- **HasSplashScreen**: Checks if splash screen is enabled
- **HasBuildVersion**: Checks if app version is specified
- **HasBuildNumber**: Checks if build number is specified
- **HasKeystorePass**: Checks if keystore password is provided
- **HasKeyaliasName**: Checks if key alias name is provided
- **HasKeyaliasPass**: Checks if key alias password is provided
- **HasKeystorePath**: Checks if keystore path is provided
- **HasStoreBuild**: Checks if store build is flagged
- **HasIosManualSign**: Checks if manual iOS signing is used
- **HasProvisionProfileIos**: Checks if iOS provisioning profile is provided
- **HasCodeSignIdentity**: Checks if iOS code signing identity is provided
- **HasProfileDevelopment**: Checks if development provisioning profile is provided
- **HasProfileDistribution**: Checks if distribution provisioning profile is provided

---

## SmartCma.Constants

Constant argument names used for command-line parsing:
- `-debug`: Enable debug mode
- `-build-location`: Build output location
- `-splash-screen`: Show splash screen
- `-build-version`: App version (e.g., "0.0.1")
- `-build-number`: CI build number (e.g., Jenkins ID)
- `-keystorepass`: Keystore password
- `-keyaliasname`: Key alias name
- `-keyaliaspass`: Key alias password
- `-keystore-path`: Path to keystore
- `-store-build`: Flag for store build
- `-code-sign-manual`: Manual code signing for iOS
- `-provision-profile`: iOS provisioning profile
- `-code-sign-identity`: iOS code sign identity
- `-profile-development`: Development provisioning profile
- `-profile-distribution`: Distribution provisioning profile

---

## Summary

SmartCma is a utility for managing build configurations via command-line arguments in Playbox CI workflows.  
It simplifies validation of required parameters and helps configure build environments dynamically.
