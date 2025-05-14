using System.Collections.Generic;

public static class IAPResponseStatus
{
    private static Dictionary<string, EIAPResponseStatus> responseStatus = new();
    
    private static bool isInitDictionary = false;
    
    public enum EIAPResponseStatus
    {
        none,
        pending,
        verified,
        unverified,
        error,
        timeout
    }
    
    private static void InitDictionary()
    {
        responseStatus["pending"] = EIAPResponseStatus.pending;
        responseStatus["verified"] = EIAPResponseStatus.verified;
        responseStatus["unverified"] = EIAPResponseStatus.unverified;
        responseStatus["error"] = EIAPResponseStatus.error;
        responseStatus["timeout"] = EIAPResponseStatus.timeout;
        
        isInitDictionary = true;
    }

    public static EIAPResponseStatus GetStatusByString(string status)
    {
        if (!isInitDictionary)
        {
            InitDictionary();
        }

        if (!responseStatus.ContainsKey(status))
            return EIAPResponseStatus.none;

        return responseStatus[status];
    }
}