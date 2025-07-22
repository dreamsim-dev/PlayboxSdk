using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        private static Dictionary<string, PurchaseData> verificationQueue = new(); // ticket_id and requestAction

        private static List<PurchaseData> keyBuffer = new();
        
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

            // if (verificationQueue.ContainsKey(productID) ||
            //     (keyBuffer.FindIndex((kv)=> kv.ProductId == productID) > -1))
            // {
            //     "purchase already exists".PlayboxWarning();
            //
            //     return;
            // }

            instance.SendRequest(productID, receipt,saveId,callback);
        }

        public void SendRequest(string productID,string receipt, string saveId, Action<bool> callback)
        {
            StartCoroutine(Request(productID,receipt, saveId, callback));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public IEnumerator Request(string productID,string receipt, string saveId, Action<bool> callback)
        {
            UnityWebRequest sendPurchaseRequest = new UnityWebRequest(uri, "POST");
        
            var sendObject = CreateSendObjectJson(productID, receipt, sendPurchaseRequest);

            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(sendObject.ToString());

            sendPurchaseRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            sendPurchaseRequest.downloadHandler = new DownloadHandlerBuffer();
        
            yield return sendPurchaseRequest.SendWebRequest();

            if (sendPurchaseRequest.result == UnityWebRequest.Result.ProtocolError ||
                sendPurchaseRequest.result == UnityWebRequest.Result.ConnectionError || 
                sendPurchaseRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                $"Request Failed: {sendPurchaseRequest.error}".PlayboxError();
            }

            if (sendPurchaseRequest.isDone)
            {
                sendPurchaseRequest.downloadHandler.text.PlayboxInfo();
            
                JObject outObject = JObject.Parse(sendPurchaseRequest.downloadHandler.text);
            
                string ticketID = outObject["ticket_id"]?.ToString();
            
                PurchaseData data = new PurchaseData
                {
                    ProductId = productID,
                    TicketId = ticketID,
                    SaveIndentifier = saveId,
                    OnValidateCallback = callback
                };            
   
                keyBuffer.Add(data);
            }
        
        }

        private JObject CreateSendObjectJson(string productID, string receipt, UnityWebRequest unityWebRequest)
        {
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SetRequestHeader("x-api-token", xApiToken);
        
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            
            JObject sendObject = new();
            
            sendObject["os_version"] = SystemInfo.operatingSystem;
            sendObject["device_name"] = SystemInfo.deviceName;
            sendObject["device_model"] = SystemInfo.deviceModel;
            sendObject["manufacturer"] = GetManufacturer();
            sendObject["device_locale"] = CultureInfo.CurrentCulture.Name;
            sendObject["time_zone"] = localZone.DisplayName;
            sendObject["app_version"] = Data.Playbox.AppVersion;
            //sendObject["build_number"] = GetBuildNumber();
            //sendObject["ram_total_and_free"] = GetDeviceRAM().ToString();
            
            
            sendObject["product_id"] = productID;
            sendObject["game_id"] = Data.Playbox.GameId;
            sendObject["version"] = Data.Playbox.AppVersion;
            //sendObject["sandbox"] = isSandbox;
            sendObject["receipt"] = receipt;

#if UNITY_ANDROID
            sendObject["platform"] = "android";
#elif UNITY_IOS
            sendObject["platform"] = "ios";
#endif
            
            return sendObject;
        }

        private IEnumerator UpdatePurchases() {

            List<string> removeFromListByTicket = new();
            
            while (true)
            {
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
                            removeFromListByTicket.Add(item.Key);
                        }
                    });
                }

                foreach (var item in removeFromListByTicket)
                {
                    verificationQueue.Remove(item);
                }
            
                removeFromListByTicket.Clear();
            
                yield return new WaitForSeconds(verifyUpdateRate);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator GetStatus(KeyValuePair<string,PurchaseData> purchaseDataItem, Action<bool> removeFromQueueCallback)
        {
            
            UnityWebRequest getStausRequest = new UnityWebRequest($"{uriStatus}/{purchaseDataItem.Key}", "GET");
        
            getStausRequest.SetRequestHeader("Content-Type", "application/json");
            getStausRequest.SetRequestHeader("x-api-token", xApiToken);
        
            getStausRequest.downloadHandler = new DownloadHandlerBuffer();
        
            yield return getStausRequest.SendWebRequest();

            if (getStausRequest.result == UnityWebRequest.Result.ProtocolError ||
                getStausRequest.result == UnityWebRequest.Result.ConnectionError ||
                getStausRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                $"Request Failed: {getStausRequest.error}".PlayboxError();
            }
            
            if (getStausRequest.isDone)
            {
                JObject json = JObject.Parse(getStausRequest.downloadHandler.text);
                
                string status = json["status"]?.ToString();
                
                switch (VerificationStatusHelper.GetStatusByString(status))
                {
                    case VerificationStatusHelper.EStatus.none:
                        
                        removeFromQueueCallback?.Invoke(true);
                    
                        break;
                
                    case VerificationStatusHelper.EStatus.pending:
                    
                        removeFromQueueCallback?.Invoke(false);
                    
                        break;
                
                    case VerificationStatusHelper.EStatus.verified:
                    
                        "Validation succeeded".PlayboxError();
                        purchaseDataItem.Value.OnValidateCallback?.Invoke(true);
                        removeFromQueueCallback?.Invoke(true);
                    
                        break;
                
                    case VerificationStatusHelper.EStatus.unverified:
                    
                        "Validation failed".PlayboxError();
                        purchaseDataItem.Value.OnValidateCallback?.Invoke(false);
                        removeFromQueueCallback?.Invoke(true);
                        
                        break;
                
                    case VerificationStatusHelper.EStatus.error:
                        
                        removeFromQueueCallback?.Invoke(true);
                        break;
                
                    case VerificationStatusHelper.EStatus.timeout:
                        
                        removeFromQueueCallback?.Invoke(true);
                        break;
                }
            
            }
        }

        private string GetManufacturer()
        {
            
#if UNITY_IOS
            return "Apple";
#endif

#if UNITY_ANDROID
            
            using (var buildClass = new AndroidJavaClass("android.os.Build"))
            {
                return buildClass.GetStatic<string>("MANUFACTURER");
            }
#endif

            return "Editor";
        }
    }
}
