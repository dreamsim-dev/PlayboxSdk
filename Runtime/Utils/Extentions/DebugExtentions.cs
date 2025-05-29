using System;
using System.Collections.Generic;
using System.Linq;
using Playbox;
using UnityEngine;
using Object = System.Object;

namespace CI.Utils.Extentions
{
    public static class DebugExtentions
    {
        private static Stack<string> prefixes = new();
        
        private static string currentPrefix = "";
        
        public static void BeginPrefixZone(string prefix)
        {
            if(!string.IsNullOrEmpty(currentPrefix)) prefixes.Push(currentPrefix);
         
            currentPrefix = prefix;
            
        }
        
        public static void EndPrefixZone()
        {
            if (prefixes.Count == 0)
            {
                currentPrefix = "";
                return;
            }
            
            if (prefixes.TryPop(out var prefix))
            {
                currentPrefix = prefix;
            }
            else
            {
                currentPrefix = "";
            }
        }

        public static void ClearPrefixes()
        {
            currentPrefix = "";
            prefixes.Clear();
        }
        
        public static void PlayboxSplashLogUGUI(this object obj)
        {
            PlayboxSplashUGUILogger.SplashEvent?.Invoke(obj.ToString());
        }
        
        public static void PlayboxSplashLog(this object obj)
        {
            PlayboxSplashLogger.SplashEvent?.Invoke(obj.ToString());
            
          //  PlayboxLogger(Color.black, text,);
        }

        private static string PlayboxLogger(Color color,object text,Action<string> action, string predicate = "Playbox",string description = "", bool isException = false)
        {
            if (!Debug.isDebugBuild) return "";
            
            string prfx = string.IsNullOrEmpty(currentPrefix) ? "" : $" <color=#{color}>[{currentPrefix}]</color> ";
            string desct = string.IsNullOrEmpty(description) ? "" : $" [{description}] ";
            string pred = string.IsNullOrEmpty(predicate) ? "" : $" [{predicate}] ";
            
            string str = $"<color=#{Color.green}>[Playbox]</color> {prfx}{pred}{desct}: {text}";
            
            action?.Invoke(str);
            
            return str;
        }

        public static string PlayboxLog(this object text, string description = "")
        {
            return PlayboxLogger(Color.white, text,str=> Debug.Log(str,text as GameObject),"Log",description);
        }
        
        public static string PlayboxError(this object text, string description = "")
        {
            return PlayboxLogger(Color.red,text,str=> Debug.LogError(str,text as GameObject),"Error", description);
        }
        
        public static string PlayboxException(this object text, string description = "")
        {
            return PlayboxLogger(Color.red,text,str=> Debug.LogException(new Exception(str),text as GameObject),"Exception",description,true);
        }
        
        public static string PlayboxWarning(this object text, string description = "")
        {
            return PlayboxLogger(Color.yellow,text,str=> Debug.LogWarning(str,text as GameObject),"Warning", description);
        }
        
        public static string PlayboxInfo(this object text, string description = "")
        {
            return PlayboxLogger(Color.gray,text,str=> Debug.Log(str,text as GameObject),"Info", description);
        }

        public static string PlayboxInitialized(this object text, string description = "")
        {
            return PlayboxLogger(Color.green,text,str=> Debug.LogError(str,text as GameObject),"Initialized", description);
        }
        
        public static string PlayboxLogD(this object text, string description = "")
        {
            PlayboxLogger(Color.white,text,str=> Debug.Log(str,text as GameObject),"Log",description);
            return text.ToString();
        }
        
        public static string PlayboxErrorD(this object text, string description = "")
        {
            PlayboxLogger(Color.red,text,str=> Debug.LogError(str,text as GameObject),"Error", description);
            return text.ToString();
        }
        
        public static string PlayboxExceptionD(this object text, string description = "")
        {
            PlayboxLogger(Color.red,text,str=> Debug.LogException(new Exception(str),text as GameObject),"Exception",description,true);
            return text.ToString();
        }
        
        public static string PlayboxWarningD(this object text, string description = "")
        {
            PlayboxLogger(Color.yellow,text,str=> Debug.LogWarning(str,text as GameObject),"Warning", description);
            return text.ToString();
        }
        
        public static string PlayboxInfoD(this object text, string description = "")
        {
            PlayboxLogger(Color.gray,text,str=> Debug.Log(str,text as GameObject),"Info", description);
            return text.ToString();
        }

        public static string PlayboxInitializedD(this object text, string description = "")
        {
            PlayboxLogger(Color.green,text,str=> Debug.LogError(str,text as GameObject),"Initialized", description);
            return text.ToString();
        }
    }
}