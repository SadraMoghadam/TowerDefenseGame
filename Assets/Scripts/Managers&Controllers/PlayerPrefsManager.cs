using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public enum PlayerPrefsKeys
    {
        LevelsInformation,
        drawProjectionLine,
        slowMotionOnExplosion,
        miniMap,
        music,
        sfx,
        ChosenLevel
    }

    public struct LevelsCompleted
    {
        public List<LevelInformation> levelsInformation;
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
    
    public void AddLevelsCompleted(int level, int stars)
    {
        LevelsCompleted levelsCompleted = new LevelsCompleted();
        List<LevelInformation> levelsList = new List<LevelInformation>();
        List<int> CompletedLevelNumbers = new List<int>();
        string jsonLevels = "";
        LevelInformation levelInformation = new LevelInformation(level, stars);
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.LevelsInformation.ToString()))
        {
            levelsList = GetLevelsCompleted();
            CompletedLevelNumbers = GetComletedLevelNumbers();
            if (CompletedLevelNumbers.Contains(levelInformation.levelNumber))
            {
                levelsList.Remove(levelInformation);
                levelInformation.stars = Math.Max(levelInformation.stars, stars);
                levelsList.Add(levelInformation);
                levelsCompleted.levelsInformation = levelsList;
                for (int i = 0; i < levelsCompleted.levelsInformation.Count; i++)
                {
                    jsonLevels += JsonUtility.ToJson(levelsCompleted.levelsInformation[i]);
                }
                PlayerPrefs.SetString(PlayerPrefsKeys.LevelsInformation.ToString(), jsonLevels);
                return;   
            }
            levelsList.Add(levelInformation);
            levelsCompleted.levelsInformation = levelsList;
            for (int i = 0; i < levelsCompleted.levelsInformation.Count; i++)
            {
                jsonLevels += JsonUtility.ToJson(levelsCompleted.levelsInformation[i]);
            }
            PlayerPrefs.SetString(PlayerPrefsKeys.LevelsInformation.ToString(), jsonLevels);
            return;
        }
        levelsList.Add(levelInformation);
        levelsCompleted.levelsInformation = levelsList;
        for (int i = 0; i < levelsCompleted.levelsInformation.Count; i++)
        {
            jsonLevels += JsonUtility.ToJson(levelsCompleted.levelsInformation[i]);
        }
        PlayerPrefs.SetString(PlayerPrefsKeys.LevelsInformation.ToString(), jsonLevels);
    }
    
    public List<LevelInformation> GetLevelsCompleted()
    {
        LevelsCompleted levelsCompleted = new LevelsCompleted();
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.LevelsInformation.ToString()))
        {
            string jsonLevels = PlayerPrefs.GetString(PlayerPrefsKeys.LevelsInformation.ToString());
            levelsCompleted.levelsInformation = new List<LevelInformation>();
            levelsCompleted.levelsInformation.Add(JsonUtility.FromJson<LevelInformation>(jsonLevels));
        }
        return levelsCompleted.levelsInformation;
    }

    public List<int> GetComletedLevelNumbers()
    {
        List<int> levelsCompleted = new List<int>();
        List<LevelInformation> levelsInformation = GetLevelsCompleted();
        for (int i = 0; i < levelsInformation.Count; i++)
        {
            levelsCompleted.Add(levelsInformation[i].levelNumber);
        }
        return levelsCompleted;
    }
    
    public int GetStarsOfLevel(int level)
    {
        int stars = 0;
        List<LevelInformation> levelsInformation = GetLevelsCompleted();
        for (int i = 0; i < levelsInformation.Count; i++)
        {
            if (level == levelsInformation[i].levelNumber)
            {
                stars = levelsInformation[i].stars;
                break;
            }
        }
        return stars;
    }
    

}
