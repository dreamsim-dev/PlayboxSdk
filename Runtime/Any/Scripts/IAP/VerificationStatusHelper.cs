using System.Collections.Generic;

public static class VerificationStatusHelper
{
    private static Dictionary<string, EStatus> responseStatus = new();
    
    private static bool isInitDictionary = false;
    
    public enum EStatus
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
        responseStatus["pending"] = EStatus.pending;
        responseStatus["verified"] = EStatus.verified;
        responseStatus["unverified"] = EStatus.unverified;
        responseStatus["error"] = EStatus.error;
        responseStatus["timeout"] = EStatus.timeout;
        
        isInitDictionary = true;
    }

    public static EStatus GetStatusByString(string status)
    {
        if (!isInitDictionary)
        {
            InitDictionary();
        }

        if (!responseStatus.ContainsKey(status))
            return EStatus.none;

        return responseStatus[status];
    }
}