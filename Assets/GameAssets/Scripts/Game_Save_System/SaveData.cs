using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public static string SAVE_DATA_KEY = "SaveData-lvl-";
    public static string LATEST_SCENE = "Latest_scene";

    [System.Serializable]
    public class PlayerData
    {
        public AgentData agentData;
        public Vector3 playerPosition;
        public Vector3 playerLookDirection;
    }

    [System.Serializable]
    public struct SavableDictElement
    {
        public string key;
        public string value;
    }


    [System.Serializable]
    public class LevelData
    {
        public int scene_ID;
        public int currentCheckpoint;

        public Dictionary<string,string> equipmentBoxes = new Dictionary<string, string>();
        public List<SavableDictElement> equipmentBoxesList = new List<SavableDictElement>();

        public Dictionary<string,string> optionalObjectives = new Dictionary<string, string>();
        public List<SavableDictElement> optionaObjList = new List<SavableDictElement>();


        public PlayerData playerData = new PlayerData();

        public void deserialize()
        {
            equipmentBoxes = ListToDict(equipmentBoxesList);
            optionalObjectives = ListToDict(optionaObjList);
        }

        public void serialize()
        {
            equipmentBoxesList = DistToList(equipmentBoxes);
            optionaObjList = DistToList(optionalObjectives);
        }
    }

    public static void SaveLevelData(int levelIndex, SaveData.LevelData data)
    {
        data.serialize();
        string save_data_json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(getLevelSaveDataKey(levelIndex),save_data_json);

        // set latest scene
        var latest_scene = PlayerPrefs.GetInt(LATEST_SCENE,-1);
        if(levelIndex > latest_scene)
        {
            PlayerPrefs.SetInt(LATEST_SCENE,levelIndex);
        }
        
    }

    public static void RemoveSaveData(int levelIndex)
    {
        PlayerPrefs.DeleteKey(getLevelSaveDataKey(levelIndex));
        var latest_scene = PlayerPrefs.GetInt(LATEST_SCENE,-1);
        if(levelIndex == latest_scene)
        {
            PlayerPrefs.DeleteKey(LATEST_SCENE);
        }
    }

    public static int GetLatestSceneID()
    {
        return PlayerPrefs.GetInt(LATEST_SCENE,-1);
    }



    // Private functions
    private static string getLevelSaveDataKey(int levelIndex)
    {
        return SAVE_DATA_KEY + levelIndex.ToString();
    }

    public static LevelData LoadLevelData(int levelIndex)
    {
        if(PlayerPrefs.HasKey(getLevelSaveDataKey(levelIndex)))
        {
            var data_string =  PlayerPrefs.GetString(getLevelSaveDataKey(levelIndex));
            var levelData =  JsonUtility.FromJson<SaveData.LevelData>(data_string);
            levelData.deserialize();
            return levelData;
        }
        return null;
    }


    private static Dictionary<string,string> ListToDict(List<SavableDictElement> dataList)
    {   var statsu_dict = new Dictionary<string,string>();
        foreach(var item in dataList)
        {
            statsu_dict[item.key] = item.value;
        }
        return statsu_dict;
    }

    private static List<SavableDictElement> DistToList(Dictionary<string,string> status_dict)
    {   
        List<SavableDictElement> dataLis = new List<SavableDictElement>();
        foreach(var item in status_dict)
        {
            var element = new SavableDictElement();
            element.key = item.Key;
            element.value = item.Value;
            dataLis.Add(element);
        }

        return dataLis;
    }

}
