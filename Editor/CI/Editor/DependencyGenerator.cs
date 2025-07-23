#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Playbox.CI
{
    public class DependencyGenerator
    {
        [MenuItem("Playbox/Resolve IOS Dependencies")]
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
    }
}

#endif