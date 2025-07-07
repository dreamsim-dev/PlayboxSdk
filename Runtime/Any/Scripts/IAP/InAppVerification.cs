using System;
using System.Collections;
using System.Collections.Generic;
using CI.Utils.Extentions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Playbox
{
    public class InAppVerification : PlayboxBehaviour
    {
        private bool isSandbox => InAppVerificationCongifuration.IsSandbox;

        [SerializeField] private float verifyUpdateRate = 0.5f;

        private const string uri = "https://api.playbox.network/verify";
        private const string uriStatus = "https://api.playbox.network/verify/status"; // uriStatus{ticket_id}
        private const string xApiToken = "plx_api_Rm8qTXe7Pzw94v1FujgEKsWD";

        private static Dictionary<string, PurchaseValidator> verificationQueue = new(); // ticket_id and requestAction

        private static List<PurchaseValidator> keyBuffer = new();
        
        private static InAppVerification instance;

        public override void Initialization()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        
            DontDestroyOnLoad(gameObject);
            
            isInitialized = true;
            StartCoroutine(UpdatePurchases());
        }
        
        public static void Validate(string productID,string receipt ,string saveId, Action<bool> callback)
        {
            if(instance == null) return;
            if(string.IsNullOrEmpty(productID)) return;
            if(string.IsNullOrEmpty(receipt)) return;
            if(callback == null) return;
        
            if (verificationQueue.ContainsKey(productID) ||
                (keyBuffer.FindIndex((kv)=> kv.ProductId == productID) > -1))
            {
                "purchase already exists".PlayboxWarning();
            
                return;
            }

            instance.SendRequest(productID, receipt,saveId,callback);
        }

        public void SendRequest(string productID,string receipt, string saveId, Action<bool> callback)
        {
            StartCoroutine(Request(productID,receipt, saveId, callback));
        }

        public IEnumerator Request(string productID,string receipt, string saveId, Action<bool> callback)
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
        
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SetRequestHeader("x-api-token", xApiToken);
        
            JObject sendObject = new JObject();
        
            sendObject["product_id"] = productID;
            sendObject["game_id"] = Data.Playbox.GameId;
            sendObject["version"] = Data.Playbox.AppVersion;
            sendObject["sandbox"] = isSandbox;
            sendObject["receipt"] = receipt;

#if UNITY_ANDROID
            sendObject["platform"] = "android";
#elif UNITY_IOS
            sendObject["platform"] = "ios";
#endif

        
            sendObject.ToString().PlayboxInfo();
        
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(sendObject.ToString());

            unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError ||
                unityWebRequest.result == UnityWebRequest.Result.ConnectionError || 
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                $"Request Failed: {unityWebRequest.error}".PlayboxError();
            }

            if (unityWebRequest.isDone)
            {
                unityWebRequest.downloadHandler.text.PlayboxInfo();
            
                JObject outObject = JObject.Parse(unityWebRequest.downloadHandler.text);
            
                string ticketID = outObject["ticket_id"]?.ToString();
            
                PurchaseValidator validator = new PurchaseValidator
                {
                    ProductId = productID,
                    TicketId = ticketID,
                    SaveIndentifier = saveId,
                    OnCallback = callback
                };            
   
                keyBuffer.Add(validator);
            }
        
        }
    
        private IEnumerator UpdatePurchases() {

            while (true)
            {
                List<string> removesProductId = new();

                foreach (var item in keyBuffer)
                {
                    verificationQueue.Add(item.TicketId, item);
                }
            
                keyBuffer.Clear();

                foreach (var item in verificationQueue)
                {
                    yield return GetStatus(item, b => { 
                        if(b)
                        {
                            removesProductId.Add(item.Key);
                        }
                    });
                }

                foreach (var item in removesProductId)
                {
                    verificationQueue.Remove(item);
                }
            
                removesProductId.Clear();
            
                yield return new WaitForSeconds(verifyUpdateRate);
            }
        }
        
        private IEnumerator GetStatus(KeyValuePair<string,PurchaseValidator> item, Action<bool> remove)
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest($"{uriStatus}/{item.Key}", "GET");
        
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SetRequestHeader("x-api-token", xApiToken);
        
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        
            yield return unityWebRequest.SendWebRequest();
        
            if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError ||
                unityWebRequest.result == UnityWebRequest.Result.ConnectionError || 
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                $"Request Failed: {unityWebRequest.error}".PlayboxError();
            }
            
            if (unityWebRequest.isDone)
            {
                unityWebRequest.downloadHandler.text.PlayboxInfo();
            
                JObject json = JObject.Parse(unityWebRequest.downloadHandler.text);

                json["status"]!.ToString().PlayboxInfo();
            
                remove?.Invoke(json["status"].ToString() != "pending");

                switch (IAPResponseStatus.GetStatusByString(json["status"].ToString()))
                {
                    case IAPResponseStatus.EIAPResponseStatus.none:
                        
                        remove?.Invoke(true);
                    
                        break;
                
                    case IAPResponseStatus.EIAPResponseStatus.pending:
                    
                        remove?.Invoke(false);
                    
                        break;
                
                    case IAPResponseStatus.EIAPResponseStatus.verified:
                    
                        item.Value.OnCallback?.Invoke(true);
                        remove?.Invoke(true);
                    
                        break;
                
                    case IAPResponseStatus.EIAPResponseStatus.unverified:
                    
                        item.Value.OnCallback?.Invoke(false);
                        remove?.Invoke(true);
                        break;
                
                    case IAPResponseStatus.EIAPResponseStatus.error:
                        
                        remove?.Invoke(true);
                        break;
                
                    case IAPResponseStatus.EIAPResponseStatus.timeout:
                        
                        remove?.Invoke(true);
                        break;
                
                    default:
                        break;
                }
            
            }
        }
    }
}
