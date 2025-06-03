using Newtonsoft.Json.Linq;

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace Playbox.SdkWindow
{
    public class DrawableWindow
    {
        public string name;
        public bool active;
        public bool hasUnsavedChanges;
        
        protected float field_width = 300;
        protected float field_height = 15;
        protected float footerSpace = 10;
        protected float headerSpace = 10;
    
        protected JObject configuration;

        public float FieldWidth
        {
            get => field_width;
            set => field_width = value;
        }

        public float FieldHeight
        {
            get => field_height;
            set => field_height = value;
        }

        public virtual void InitName()
        {
            configuration = new JObject();
        }

        public virtual void HasRenderToggle()
        {
            active = EditorGUILayout.Toggle(name, active);
            GUILayout.Space(5);
        }

        public virtual void Title()
        {
            //GUILayout.Label(name);
        }

        public virtual void Header()
        {
            GUILayout.Space(headerSpace);
        }

        public virtual void Body()
        {
        }

        public virtual void Footer()
        {
            GUILayout.Space(footerSpace);
        }

        public virtual void Save()
        {
        }

        public virtual void Load()
        {
        }
    }
}

#endif