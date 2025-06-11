using System.Collections.Generic;
using UnityEngine;

namespace Playbox
{
    public class CrossPromoButton: MonoBehaviour
    {
        [SerializeField]
        private string promotedID = "world.dreamsim.slumdogbillionaire";
        
        [SerializeField]
        private string campaign = "dreamsim";
        
        public void Click()
        {
            Dictionary<string,string> properties = new ();
            
            properties.Add("campaign", campaign);
            properties.Add("promoted_id", promotedID);
            
            CrossPromo.RecordCrossPromoImpression(promotedID,campaign, properties);
        }
    }
}