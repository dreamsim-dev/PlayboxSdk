using System;
using UnityEngine;
using UnityEngine.Events;
using Utils.Timer;

namespace Playbox
{
    public class PlayboxSplashUGUILogger : PlayboxBehaviour
    {
        public static UnityAction<string> SplashEvent;

        [SerializeField]
        private string text = "";
        
        [SerializeField]
        private GUIStyle style = new GUIStyle();
        private Texture2D texture;

        private float splashTime = 5;
        
        private PlayboxTimer timer;
        private bool isEnabled = false;


        public override void Initialization()
        {
            Init();
        }

        private void Init()
        {
            SplashEvent += OnText;
            
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            
            style.fontStyle = FontStyle.Normal;
            style.fontSize = 36;
            style.normal.background = texture;
            
            timer = new PlayboxTimer();
            timer.initialTime = splashTime;

            timer.OnTimeRemaining += f =>
            {


            };
            
            timer.OnTimerStart += () =>
            {
                isEnabled = true;

            };
            
            timer.OnTimeOut += () =>
            {
                isEnabled = false;
            };
        }

        private void OnText(string text)
        {
            this.text = text;
            timer.Start();
        }

        private void OnGUI()
        {
            if(!isEnabled)
                return;
            
            var rect = style.CalcSize(new GUIContent(text));
            
            GUI.Label(new Rect(new Vector2(200,200), rect), text, style);
        }

        private void Update()
        {
            timer.Update(Time.deltaTime);
        }
    }
}