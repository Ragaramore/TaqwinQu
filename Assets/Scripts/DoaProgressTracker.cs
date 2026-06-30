using UnityEngine;

public static class DoaProgressTracker
{
    private const string CompletedStarsKey = "TaqwinquCompletedDoaStars";
    private const string CompletedDoaPrefix = "TaqwinquCompletedDoa_";
    private const string RegisteredTotalDoaCountKey = "TaqwinquRegisteredTotalDoaCount";

    public static void RegisterTotalDoaCount(int totalDoaCount)
    {
        if (totalDoaCount <= 0)
        {
            return;
        }

        int savedCount = PlayerPrefs.GetInt(RegisteredTotalDoaCountKey, 0);
        if (totalDoaCount > savedCount)
        {
            PlayerPrefs.SetInt(RegisteredTotalDoaCountKey, totalDoaCount);
            PlayerPrefs.Save();
        }
    }

    public static int GetRegisteredTotalDoaCount(int fallbackTotalDoaCount = 0)
    {
        int savedCount = PlayerPrefs.GetInt(RegisteredTotalDoaCountKey, 0);
        return Mathf.Max(savedCount, Mathf.Max(0, fallbackTotalDoaCount));
    }

    public static void MarkDoaCompleted(DoaData doaData)
    {
        string progressKey = GetDoaKey(doaData);
        if (string.IsNullOrEmpty(progressKey))
        {
            return;
        }

        string prefKey = CompletedDoaPrefix + progressKey;
        if (PlayerPrefs.GetInt(prefKey, 0) == 1)
        {
            return;
        }

        PlayerPrefs.SetInt(prefKey, 1);
        PlayerPrefs.SetInt(CompletedStarsKey, GetCompletedStarCount() + 1);
        PlayerPrefs.Save();
    }

    public static bool HasCompletedDoa(DoaData doaData)
    {
        string progressKey = GetDoaKey(doaData);
        if (string.IsNullOrEmpty(progressKey))
        {
            return false;
        }

        return PlayerPrefs.GetInt(CompletedDoaPrefix + progressKey, 0) == 1;
    }

    public static int GetCompletedStarCount()
    {
        return Mathf.Max(0, PlayerPrefs.GetInt(CompletedStarsKey, 0));
    }

    private static string GetDoaKey(DoaData doaData)
    {
        if (doaData == null)
        {
            return string.Empty;
        }

        string rawKey = !string.IsNullOrEmpty(doaData.name) ? doaData.name : doaData.namaDoa;
        if (string.IsNullOrEmpty(rawKey))
        {
            return string.Empty;
        }

        rawKey = rawKey.Trim().ToLowerInvariant();
        return rawKey.Replace(' ', '_');
    }
}
