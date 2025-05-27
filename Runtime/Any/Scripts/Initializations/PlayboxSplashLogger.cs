using TMPro;
using UnityEngine.Events;

namespace Playbox
{
    public class PlayboxSplashLogger : PlayboxBehaviour
    {
        public static UnityAction<string> SplashEvent;
        
        TMP_Text text;

        private void Start()
        {
            text = GetComponent<TMP_Text>();

            SplashEvent += OnText;
        }

        private void OnText(string text)
        {
            this.text.text = text;
        }
    }
}