using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public string levelMainText;
    public string levelSubText;
    public string LevelDescription;
    public string levelImagePath;
}

[System.Serializable]
public class LevelListObject
{
    //public List<Level> levelList { get; set; }
    public List<Level> levelList;
}
