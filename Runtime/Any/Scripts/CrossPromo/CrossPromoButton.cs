using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using CI.Utils.Extentions;
using UnityEngine;

namespace Playbox
{
    public class CrossPromoButton: MonoBehaviour
    {
        [SerializeField]
        private string Link = "";
        
        [SerializeField]
        private string promotedID = "";
        
        [SerializeField]
        private string campaign = "";
        
        private void OnEnable()
        {
            CrossPromo.OnInviteLinkGenerated += s =>
            {
                s.PlayboxInfo("LINK");
                s.PlayboxSplashLogUGUI();
            };
            CrossPromo.OnOpenStoreLinkGenerated += s =>
            {
                s.PlayboxInfo("LINK");
                s.PlayboxSplashLogUGUI();
                Application.OpenURL(s);
            };
        }

        public void Click()
        {
            Dictionary<string,string> properties = new ();
            
            properties.Add("campaign", campaign);
            properties.Add("promoted_id", promotedID);
            
            CrossPromo.OpenStore(promotedID,campaign, properties,this);
        }

        public void GenerateLink()
        {
            Dictionary<string,string> properties = new ();
            
            properties.Add("campaign", campaign);
            properties.Add("promoted_id", promotedID);
            
            CrossPromo.GenerateUserInviteLink(properties);
        }
    }
}