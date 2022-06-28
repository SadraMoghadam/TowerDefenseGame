using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public enum PlayerPrefsKeys
    {
        LevelsCompleted,
        drawProjectionLine,
        slowMotionOnExplosion,
        miniMap,
        music,
        sfx,
        ChosenLevel
    }

    public struct LevelsCompleted
    {
        public List<int> levelsCompleted;
    }

    public void SetBool(PlayerPrefsKeys playerPrefsKeys, bool value)
    {
        PlayerPrefs.SetInt(playerPrefsKeys.ToString(), value ? 1 : 0);
    }
    
    public bool GetBool(PlayerPrefsKeys playerPrefsKeys, bool defaultValue = true)
    {
        int value = defaultValue ? 1 : 0;
        if (PlayerPrefs.HasKey(playerPrefsKeys.ToString()))
        {
            value = PlayerPrefs.GetInt(playerPrefsKeys.ToString());   
        }
        return value == 1 ? true : false;
    }
    
    public void SetFloat(PlayerPrefsKeys playerPrefsKeys, float value)
    {
        PlayerPrefs.SetFloat(playerPrefsKeys.ToString(), value);
    }
    
    public float GetFloat(PlayerPrefsKeys playerPrefsKeys, float defaultValue)
    {
        float value = defaultValue;
        if (PlayerPrefs.HasKey(playerPrefsKeys.ToString()))
        {
            value = PlayerPrefs.GetFloat(playerPrefsKeys.ToString());   
        }
        return value;
    }
    
    public void SetInt(PlayerPrefsKeys playerPrefsKeys, int value)
    {
        PlayerPrefs.SetInt(playerPrefsKeys.ToString(), value);
    }
    
    public int GetInt(PlayerPrefsKeys playerPrefsKeys, int defaultValue)
    {
        int value = defaultValue;
        if (PlayerPrefs.HasKey(playerPrefsKeys.ToString()))
        {
            value = PlayerPrefs.GetInt(playerPrefsKeys.ToString());   
        }
        return value;
    }
    
    public void AddLevelsCompleted(int level)
    {
        LevelsCompleted levelsCompleted = new LevelsCompleted();
        List<int> levelsList = new List<int>();
        string jsonLevels = "";
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.LevelsCompleted.ToString()))
        {
            levelsList = GetLevelsCompleted();
            if (levelsList.Contains(level))
            {
                return;   
            }
            levelsList.Add(level);
            levelsCompleted.levelsCompleted = levelsList;
            jsonLevels = JsonUtility.ToJson(levelsCompleted);
            PlayerPrefs.SetString(PlayerPrefsKeys.LevelsCompleted.ToString(), jsonLevels);
            return;
        }
        levelsList.Add(level);
        levelsCompleted.levelsCompleted = levelsList;
        jsonLevels = JsonUtility.ToJson(levelsCompleted);
        PlayerPrefs.SetString(PlayerPrefsKeys.LevelsCompleted.ToString(), jsonLevels);
    }
    
    public List<int> GetLevelsCompleted()
    {
        LevelsCompleted levelsCompleted = new LevelsCompleted();
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.LevelsCompleted.ToString()))
        {
            string jsonLevels = PlayerPrefs.GetString(PlayerPrefsKeys.LevelsCompleted.ToString());
            levelsCompleted = JsonUtility.FromJson<LevelsCompleted>(jsonLevels);
        }
        return levelsCompleted.levelsCompleted;
    }
    
}
