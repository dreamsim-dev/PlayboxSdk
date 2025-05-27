using System;
using UnityEngine;
using UnityEngine.Events;

namespace Playbox
{
    public class PlayboxSplashLogger : PlayboxBehaviour
    {
        [SerializeField]
        private UnityAction<string> OnSplash;
        
        public static UnityAction<string> SplashEvent;

        private void Start()
        {
            SplashEvent += (arg0) => OnSplash?.Invoke(arg0);
        }
    }
}