using System;

namespace Playbox
{
    public class PlayboxSplashLogger : PlayboxBehaviour
    {
        public static  Action<string> SplashEvent;
        
        public override void Initialization()
        {
            SplashEvent += Splash;
        }

        private void Splash(string message)
        {
            
        }
    }
}