using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInformation
{
    public int levelNumber;
    public int stars;

    public LevelInformation(int levelNumber, int stars)
    {
        this.levelNumber = levelNumber;
        this.stars = stars;
    }
    
}
