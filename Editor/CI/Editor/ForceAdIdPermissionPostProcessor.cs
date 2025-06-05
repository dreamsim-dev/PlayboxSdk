namespace Playbox.CI
{
#if UNITY_ANDROID
    using System.IO;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Android;

    public class ForceAdIdPermissionPostProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 1000;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");

            if (!File.Exists(manifestPath))
            {
                UnityEngine.Debug.LogError("AndroidManifest.xml не найден: " + manifestPath);
                return;
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(manifestPath);

            var manifestNode = xmlDoc.SelectSingleNode("/manifest");

            if (manifestNode == null)
            {
                UnityEngine.Debug.LogError("Не удалось найти <manifest> в AndroidManifest.xml");
                return;
            }

            var ns = manifestNode.GetNamespaceOfPrefix("android");
            var permissionName = "com.google.android.gms.permission.AD_ID";
            
            foreach (XmlNode node in manifestNode.ChildNodes)
            {
                if (node.Name == "uses-permission" &&
                    node.Attributes != null &&
                    node.Attributes["android:name"]?.Value == permissionName)
                {
                    UnityEngine.Debug.Log("Разрешение AD_ID уже существует в AndroidManifest.xml");
                    return;
                }
            }
            
            var permissionNode = xmlDoc.CreateElement("uses-permission");
            var attr = xmlDoc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
            attr.Value = permissionName;
            permissionNode.Attributes.Append(attr);
            manifestNode.InsertBefore(permissionNode, manifestNode.FirstChild);

            xmlDoc.Save(manifestPath);
            UnityEngine.Debug.Log("Добавлено разрешение: " + permissionName);
        }
    }
#endif

}