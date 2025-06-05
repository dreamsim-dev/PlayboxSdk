#if UNITY_ANDROID
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class ForceAdIdPermissionPostProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 999;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
            return;

        // Для .aab путь отличается
        string manifestPath = Path.Combine(report.summary.outputPath, "src", "main", "AndroidManifest.xml");

        if (!File.Exists(manifestPath))
        {
            UnityEngine.Debug.LogWarning("AndroidManifest.xml не найден: " + manifestPath);
            return;
        }

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(manifestPath);

        var manifestNode = xmlDoc.SelectSingleNode("/manifest");
        if (manifestNode == null)
        {
            UnityEngine.Debug.LogError("Не найден тег <manifest> в AndroidManifest.xml");
            return;
        }

        const string permissionName = "com.google.android.gms.permission.AD_ID";

        // Проверка — существует ли уже разрешение
        foreach (XmlNode node in manifestNode.ChildNodes)
        {
            if (node.Name == "uses-permission" &&
                node.Attributes?["android:name"]?.Value == permissionName)
            {
                UnityEngine.Debug.Log("Разрешение AD_ID уже есть в манифесте.");
                return;
            }
        }

        // Добавление разрешения
        XmlElement permissionElement = xmlDoc.CreateElement("uses-permission");
        XmlAttribute nameAttr = xmlDoc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
        nameAttr.Value = permissionName;
        permissionElement.Attributes.Append(nameAttr);
        manifestNode.AppendChild(permissionElement);

        xmlDoc.Save(manifestPath);
        UnityEngine.Debug.Log("Добавлено разрешение AD_ID в манифест.");
    }
}
#endif