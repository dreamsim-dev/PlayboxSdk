using Playbox.SdkConfigurations;

#if UNITY_EDITOR

using UnityEngine;

namespace Playbox.SdkWindow
{
    public class TestableWindow : DrawableWindow
    {
        public override void Body()
        {
            if (!active)
                return;
        
            name = nameof(TestableWindow);
        
            GUILayout.Label(name);
        
            if (GUILayout.Button("Load", GUILayout.Height(30), GUILayout.Width(100)))
            {
                GlobalPlayboxConfig.Load();
            }
        
            if (GUILayout.Button("Save", GUILayout.Height(30), GUILayout.Width(100)))
            {
                GlobalPlayboxConfig.Save();
            }
        }
    }
}

#endif