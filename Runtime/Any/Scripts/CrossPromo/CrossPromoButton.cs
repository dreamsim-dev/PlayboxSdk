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
        private string promotedID = "world.dreamsim.slumdogbillionaire";
        
        [SerializeField]
        private string campaign = "dreamsim";


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
            };
        }

        public void Click()
        {
            Dictionary<string,string> properties = new ();
            
            properties.Add("campaign", campaign);
            properties.Add("promoted_id", promotedID);
            
            CrossPromo.RecordCrossPromoImpression(promotedID,campaign, properties);
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